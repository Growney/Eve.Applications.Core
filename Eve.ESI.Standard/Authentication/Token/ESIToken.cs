using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication;
using Eve.ESI.Standard.Authentication.Configuration;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.Authentication.Token
{
    public class ESIToken : StoredObjectBase
    {

        public override long Id { get; set; }
        private AuthenticationToken m_token;
        private TokenCharacterInfo m_character;

        public eESIEntityType EntityType { get; private set; }
        public long CharacterID
        {
            get
            {
                return m_character?.CharacterID ?? 0;
            }
        }
        public int AllianceID { get; private set; }
        public int CorporationID { get; private set; }

        public long EntityID
        {
            get
            {
                long retVal = 0;
                switch (EntityType)
                {
                    case eESIEntityType.character:
                        retVal = CharacterID;
                        break;
                    case eESIEntityType.corporation:
                        retVal = CorporationID;
                        break;
                    case eESIEntityType.alliance:
                        retVal = AllianceID;
                        break;
                    default:
                        break;
                }
                return retVal;
            }
        }
        public bool RequiresRefresh
        {
            get
            {
                return m_character?.ExpiresOn < DateTime.UtcNow;
            }
        }
        private HashSet<eESIScope> m_scopes;
        public ESIToken()
        {

        }

        private ESIToken(TokenCharacterInfo character, AuthenticationToken token)
        {
            m_token = token;
            m_character = character;
        }

        public override IDataCommand CreateDeleteCommand()
        {
            throw new NotImplementedException();
        }

        public override IDataCommand CreateLoadFromPrimaryKey(long primaryKey)
        {
            DataCommand command = new DataCommand("ESIToken", "Single");
            command.AddParameter("ID", System.Data.DbType.Int64).Value = primaryKey;
            return command;
        }

        public override IDataCommand CreateSaveCommand()
        {
            DataCommand command = new DataCommand("ESIToken", "Save");
            command.AddParameter("Id", System.Data.DbType.Int32).Value = Id;
            command.AddParameter("AccessToken", System.Data.DbType.String).Value = m_token.Access_Token;
            command.AddParameter("TokenType", System.Data.DbType.String).Value = m_token.Token_Type;
            command.AddParameter("ExpiresIn", System.Data.DbType.Int32).Value = m_token.Expires_In;
            command.AddParameter("RefreshToken", System.Data.DbType.String).Value = m_token.Refresh_Token ?? String.Empty;

            command.AddParameter("EntityType", System.Data.DbType.Byte).Value = (byte)EntityType;
            command.AddParameter("CharacterID", System.Data.DbType.Int64).Value = CharacterID;
            command.AddParameter("CorporationID", System.Data.DbType.Int32).Value = CorporationID;
            command.AddParameter("AllianceID", System.Data.DbType.Int32).Value = AllianceID;

            command.AddParameter("ExpiresOn", System.Data.DbType.DateTime).Value = m_character.ExpiresOn;
            command.AddParameter("Scopes", System.Data.DbType.String).Value = m_character.Scopes ?? String.Empty;
            command.AddParameter("CharacterOwnerHash", System.Data.DbType.String).Value = m_character.CharacterOwnerHash;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            m_token = new AuthenticationToken()
            {
                Access_Token = adapter.GetValue("AccessToken", string.Empty),
                Token_Type = adapter.GetValue("TokenType", string.Empty),
                Expires_In = adapter.GetValue("ExpiresIn", 0),
                Refresh_Token = adapter.GetValue("RefreshToken", string.Empty)
            };

            m_character = new TokenCharacterInfo()
            {
                CharacterID = adapter.GetValue("CharacterID", 0L),
                ExpiresOn = adapter.GetValue("ExpiresOn", DateTime.MinValue),
                Scopes = adapter.GetValue("Scopes", string.Empty),
                CharacterOwnerHash = adapter.GetValue("CharacterOwnerHash", string.Empty)
            };

            EntityType = (eESIEntityType)adapter.GetValue("EntityType", 0);
            CorporationID = adapter.GetValue("CorporationID", 0);
            AllianceID = adapter.GetValue("AllianceID", 0);
            m_scopes = GetScopesSet(m_character.Scopes);
        }

        public ESIToken Copy(eESIEntityType newEntityType)
        {
            eESIScope[] newScopeSet = new eESIScope[m_scopes.Count];
            m_scopes.CopyTo(newScopeSet);

            return new ESIToken(
                new TokenCharacterInfo()
                {
                    CharacterID = m_character.CharacterID,
                    ExpiresOn = m_character.ExpiresOn,
                    Scopes = m_character.Scopes,
                    CharacterOwnerHash = m_character.CharacterOwnerHash
                },
                new AuthenticationToken()
                {
                    Access_Token = m_token.Access_Token,
                    Token_Type = m_token.Token_Type,
                    Expires_In = m_token.Expires_In,
                    Refresh_Token = m_token.Refresh_Token
                })
            {
                EntityType = newEntityType,
                CorporationID = this.CorporationID,
                AllianceID = this.AllianceID,
                m_scopes = new HashSet<eESIScope>(newScopeSet)
            };
        }

        public async Task<ESITokenRefreshResponse> GetAuthenticationToken(IESIAuthenticatedConfig config, ICommandController controller)
        {
            ESITokenRefreshResponse retVal;
            if (RequiresRefresh && !String.IsNullOrWhiteSpace(m_token.Refresh_Token))
            {
                retVal = await config.Client.RefreshToken( m_token.Refresh_Token);
                if (retVal.Status.HasFlag(eESITokenRefreshResponseStatus.Success))
                {
                    TokenCharacterInfo characterInfo = await config.Client.VerifyToken(retVal.Token);
                    if (characterInfo != null)
                    {
                        if (m_character.CharacterOwnerHash.Equals(characterInfo.CharacterOwnerHash))
                        {
                            m_token.Access_Token = retVal.Token;
                            m_token.Expires_In = retVal.ExpiresIn;
                            m_character.ExpiresOn = characterInfo.ExpiresOn;
                            Save(controller);
                        }
                        else
                        {
                            retVal = new ESITokenRefreshResponse(eESITokenRefreshResponseStatus.OwnerChange);
                        }
                        
                    }
                    else
                    {
                        retVal = new ESITokenRefreshResponse(eESITokenRefreshResponseStatus.FailedToVerify);
                    }
                }
            }
            else
            {
                retVal = new ESITokenRefreshResponse(m_token.Access_Token,m_token.Expires_In);
            }
            return retVal;
        }

        public static HashSet<eESIScope> GetScopesSet(string scopes)
        {
            HashSet<eESIScope> retval = new HashSet<eESIScope>();
            if (!string.IsNullOrWhiteSpace(scopes))
            {
                string[] scopeSplit = scopes.Split(' ');
                foreach (string scope in scopeSplit)
                {
                    if (Enum.TryParse<eESIScope>(ESIScopeHelper.GetScopeEnumString(scope), out eESIScope result))
                    {
                        if (!retval.Contains(result))
                        {
                            retval.Add(result);
                        }
                    }
                }
            }
            return retval;
        }

        public static List<ESIToken> GetAll(ICommandController controller)
        {
            DataCommand command = new DataCommand("ESIToken", "All");
            return Load<ESIToken>(controller.ExecuteCollectionCommand(command));
        }
        public static List<ESIToken> ForEntityType(ICommandController controller, eESIEntityType type)
        {
            DataCommand command = new DataCommand("ESIToken", "ForEntityType");
            command.AddParameter("EntityType", System.Data.DbType.Byte).Value = (byte)type;
            return Load<ESIToken>(controller.ExecuteCollectionCommand(command));
        }
        public static List<ESIToken> ForEntityTypeAndID(ICommandController controller,long entityID,eESIEntityType type)
        {
            DataCommand command = new DataCommand("ESIToken", "ForEntityTypeAndID");
            command.AddParameter("EntityID", System.Data.DbType.Int64).Value = entityID;
            command.AddParameter("EntityType", System.Data.DbType.Byte).Value = (byte)type;
            return Load<ESIToken>(controller.ExecuteCollectionCommand(command));
        }
        public static List<ESIToken> GetForScope(ICommandController controller,long entityID,eESIEntityType type,eESIScope scope)
        {
            return GetForScope(controller, new List<long>(), entityID, type, scope);
        }
        public static List<ESIToken> GetForScope(ICommandController controller, IEnumerable<long> ids, long entityID, eESIEntityType type, eESIScope scope)
        {
            DataCommand command = new DataCommand("ESIToken", "ForScope");
            command.AddParameter("Scope", System.Data.DbType.String).Value = ESIScopeHelper.GetScopeString(scope);
            command.AddParameter("Ids", "LongIDList").Value = ids.CreateIDList();
            command.AddParameter("EntityID", System.Data.DbType.Int64).Value = entityID;
            command.AddParameter("EntityType", System.Data.DbType.Byte).Value = (byte)type;
            return Load<ESIToken>(controller.ExecuteCollectionCommand(command));
        }
        public static List<ESIToken> GetForID(ICommandController controller, IEnumerable<long> ids)
        {
            DataCommand command = new DataCommand("ESIToken", "ForId");
            command.AddParameter("Ids", "LongIDList").Value = ids.CreateIDList();
            return Load<ESIToken>(controller.ExecuteCollectionCommand(command));
        }
        public static async Task<ESIToken> Authenticate(IESIAuthenticatedConfig config, ICommandController controller, eESIEntityType type, string code)
        {
            ESIToken retVal = null;
            AuthenticationToken token = await config.Client.RequestToken(code);
            if (token != null)
            {
                TokenCharacterInfo info = await config.Client.VerifyToken(token.Access_Token);
                if (info != null)
                {
                    ESICallResponse<DataItem.Character.CharacterInfo> characterInfo = await DataItem.Character.CharacterInfo.GetCharacterInfo(config.Client, controller, info.CharacterID);
                    if (characterInfo.IsSuccess)
                    {
                        retVal = new ESIToken()
                        {
                            EntityType = type,
                            m_token = token,
                            m_character = info,
                            m_scopes = GetScopesSet(info.Scopes),
                            CorporationID = characterInfo.Data.CorporationId,
                            AllianceID = characterInfo.Data.AllianceId
                        };
                    }
                }
            }
            return retVal;
        }

        public static List<ESIToken> FindWithScope(IEnumerable<ESIToken> tokens, eESIScope scope)
        {
            List<ESIToken> retval = new List<ESIToken>();

            foreach (ESIToken token in tokens)
            {
                if(token.m_scopes != null)
                {
                    if (token.m_scopes.Contains(scope))
                    {
                        retval.Add(token);
                    }
                }

            }

            return retval;

        }
    }
}
