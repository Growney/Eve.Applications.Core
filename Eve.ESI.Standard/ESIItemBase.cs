using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard
{
    public abstract class ESIItemBase : StoredObjectBase
    {
        protected const bool c_alwaysReturnOldData = false;
        public override long Id { get; set; }
        public Guid CallID { get; internal set; }

        public ESIItemBase()
        {

        }

        public override IDataCommand CreateSaveCommand()
        {
            IDataCommand command = OnCreateSaveCommand();
            command.AddParameter("CallID", System.Data.DbType.Guid).Value = CallID;
            return command;
        }


        public override IDataCommand CreateLoadFromPrimaryKey(long primaryKey)
        {
            throw new NotImplementedException();
        }
        public override IDataCommand CreateDeleteCommand()
        {
            throw new NotImplementedException();
        }
        public abstract IDataCommand OnCreateSaveCommand();
        protected override void OnLoad(IDataAdapter adapter)
        {
            CallID = new Guid(adapter.GetValue("CallID", string.Empty));
        }
        protected static bool ShouldReRunResponse(ESICallResponse response,bool alwaysReturnOldData)
        {
            return response == null || (response.ReRun && !alwaysReturnOldData);
        }
        protected static void ClearStoredData<T>(ICommandController controller,Dictionary<string,object> values = null, Dictionary<string, string> queryParameters = null) where T : ESIItemBase
        {
            ESICallParameters parameters = new ESICallParameters(template: ESIItemAttribute.GetRouteTemplate(typeof(T)), tokenValue: values, queryParameters: queryParameters);
            ESICallResponse<T> result = ESICallResponse.GetResponseForParameterHash<ESICallResponse<T>>(controller, parameters.GetGuid());
            ESICallResponse<T>.ExecuteClearDataCommand<T>(controller, result.CallID);
        }
        protected static async System.Threading.Tasks.Task<ESICallResponse<T>> GetItem<T>(IESIAuthenticationClient authClient, ICommandController controller, Dictionary<string, object> values = null, Dictionary<string, string> queryParameters = null, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask = null, bool alwaysReturnOldData = c_alwaysReturnOldData) where T : ESIItemBase, new()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            ESICallParameters parameters = new ESICallParameters(template: ESIItemAttribute.GetRouteTemplate(typeof(T)), tokenValue: values, queryParameters: queryParameters);
            ESICallResponse<T> result = null;
            if (controller != null)
            {
                result = ESICallResponse.GetResponseForParameterHash<ESICallResponse<T>>(controller, parameters.GetGuid());
            }
            if (ShouldReRunResponse(result,alwaysReturnOldData))
            {
                bool authenticated = true;
                if(authenticationTokenTask != null)
                {
                    ESITokenRefreshResponse response = await authenticationTokenTask();
                    if (response.Status.HasFlag(eESITokenRefreshResponseStatus.Success))
                    {
                        parameters.Token = response.Token;
                    }
                    else
                    {
                        authenticated = false;
                    }
                }
                if (authenticated)
                {
                    if (result != null)
                    {
                        parameters.IfNoneMatch = result.ETag;
                    }
                    result = await authClient.GetSingleResponseAsync<ESICallResponse<T>,T>(result?.CallID ?? Guid.NewGuid(), parameters);
                    if (result != null)
                    {
                        if (controller != null)
                        {
                            if (result.ResponseCode == System.Net.HttpStatusCode.OK)
                            {
                                ESICallResponse.ExecuteClearDataCommand<T>(controller, result.CallID);
                            }

                            result.Save(controller);

                            if (result.ResponseCode != System.Net.HttpStatusCode.OK)
                            {
                                ESICallResponse<T>.SetStoredData<T>(controller, result);
                            }
                        }
                    }
                }
                else
                {
                    result = ESICallResponse<T>.CreateForCode<ESICallResponse<T>,T>(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                if (controller != null)
                {
                    ESICallResponse<T>.SetStoredData<T>(controller, result);
                }
            }
            if (result != null)
            {
                result.TimeTaken = stopwatch.ElapsedMilliseconds;
            }
            return result;
        }
   
        protected static async System.Threading.Tasks.Task<ESICollectionCallResponse<T>> GetCollection<T>(IESIAuthenticationClient apiClient, ICommandController controller, Dictionary<string, object> values, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask = null, bool alwaysReturnOldData = c_alwaysReturnOldData) where T : ESIItemBase, new()
        {
            return await GetCollection<T>(apiClient, controller, values, authenticationTokenTask, -1, alwaysReturnOldData);
        }
        private static async System.Threading.Tasks.Task<ESICollectionCallResponse<T>> GetCollection<T>(IESIAuthenticationClient apiClient, ICommandController controller, Dictionary<string, object> values, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask = null, int page = -1, bool alwaysReturnOldData = c_alwaysReturnOldData) where T : ESIItemBase, new()
        {
            ESICallParameters parameters = new ESICallParameters(template: ESIItemAttribute.GetRouteTemplate(typeof(T)), tokenValue: values, page: page);
            ESICollectionCallResponse<T> result = ESICallResponse.GetResponseForParameterHash<ESICollectionCallResponse<T>>(controller, parameters.GetGuid());
            if (ShouldReRunResponse(result, alwaysReturnOldData))
            {
                bool authenticated = true;
                if (authenticationTokenTask != null)
                {
                    ESITokenRefreshResponse response = await authenticationTokenTask();
                    if (response.Status.HasFlag(eESITokenRefreshResponseStatus.Success))
                    {
                        parameters.Token = response.Token;
                    }
                    else
                    {
                        authenticated = false;
                    }
                }
                if (authenticated)
                {
                    if (result != null)
                    {
                        parameters.IfNoneMatch = result.ETag;
                    }
                    result = await apiClient.GetCollectionResponseAsync<ESICollectionCallResponse<T>,T>(result?.CallID ?? Guid.NewGuid(), parameters);
                    if (result != null)
                    {
                        if (result.ResponseCode == System.Net.HttpStatusCode.OK)
                        {
                            ESICallResponse.ExecuteClearDataCommand<T>(controller, result.CallID);
                        }

                        result.Save(controller);

                        if (result.ResponseCode != System.Net.HttpStatusCode.OK)
                        {
                            ESICollectionCallResponse<T>.SetStoredData<T>(controller, result);
                        }
                    }
                }
                else
                {
                    result = ESICollectionCallResponse<T>.CreateForCode<ESICollectionCallResponse<T>, T>(System.Net.HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                ESICollectionCallResponse<T>.SetStoredData<T>(controller, result);
            }
            return result;
        }
        internal protected static async System.Threading.Tasks.Task<ESIPagedCallResponse<T>> GetPages<T>(IESIAuthenticationClient authClient, ICommandController controller, Dictionary<string, object> values, Func<Task<ESITokenRefreshResponse>> authenticationToken = null) where T : ESIItemBase, new()
        {
            ESIPagedCallResponse<T> retVal = new ESIPagedCallResponse<T>();
            ESICollectionCallResponse<T> pageOne = await GetCollection<T>(authClient, controller, values, authenticationToken, 1);
            retVal.AddPage(pageOne);
            if (pageOne.Pages > 0)
            {
                for (int i = 2; i <= pageOne.Pages; i++)
                {
                    ESICollectionCallResponse<T> nextPage = await GetCollection<T>(authClient, controller, values, authenticationToken, i);
                    retVal.AddPage(nextPage);
                }
            }
            return retVal;
        }
        

    }
}
