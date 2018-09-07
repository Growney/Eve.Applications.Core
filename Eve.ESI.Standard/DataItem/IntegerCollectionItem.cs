using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem
{
    public abstract class IntegerCollectionItem : ESIItemBase
    {
        public long Value { get; internal set; }

        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("IntegerList", "Save");
            command.AddParameter("Value", System.Data.DbType.Int64).Value = Value;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            Value = adapter.GetValue("Value", 0);
        }

        protected static async System.Threading.Tasks.Task<ESIIntegerCollectionCallResponse<T>> GetIntegerCollection<T>(IESIAuthenticationClient client, ICommandController controller, Dictionary<string, object> values = null, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask = null, bool alwaysReturnOldData = c_alwaysReturnOldData) where T : IntegerCollectionItem, new()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            ESICallParameters parameters = new ESICallParameters(template: ESIItemAttribute.GetRouteTemplate(typeof(T)), tokenValue: values);
            ESIIntegerCollectionCallResponse<T> result = null;
            if (controller != null)
            {
                result = ESICallResponse.GetResponseForParameterHash<ESIIntegerCollectionCallResponse<T>>(controller, parameters.GetGuid());
            }
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
                    result = await client.GetIntegerCollectionResponse<ESIIntegerCollectionCallResponse<T>,T>(result?.CallID ?? Guid.NewGuid(), parameters);
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
                                ESIIntegerCollectionCallResponse<T>.SetStoredData<T>(controller, result);
                            }
                        }
                    }
                }
                else
                {
                    result = ESICollectionCallResponse<T>.CreateForCode<ESIIntegerCollectionCallResponse<T>, T>(System.Net.HttpStatusCode.Unauthorized);
                }

            }
            else
            {
                if (controller != null)
                {
                    ESIIntegerCollectionCallResponse<T>.SetStoredData<T>(controller, result);
                }
            }
            if (result != null)
            {
                result.TimeTaken = stopwatch.ElapsedMilliseconds;
            }
            return result;
        }
    }

    public static class IntegerCollectionItemExtensionMethods
    {
        public static IEnumerable<long> ToIntegerList(this IEnumerable<IntegerCollectionItem> list)
        {
            List<long> retVal = new List<long>();
            foreach(IntegerCollectionItem item in list)
            {
                retVal.Add(item.Value);
            }
            return retVal;
        }
        public static IEnumerable<T> ToIntegerCollection<T>(this IEnumerable<long> list, Guid callID) where T : IntegerCollectionItem, new()
        {
            List<T> retVal = new List<T>();
            foreach (int item in list)
            {
                retVal.Add(new T()
                {
                    CallID = callID,
                    Value = item
                });
            }
            return retVal;
        }
    }
}
