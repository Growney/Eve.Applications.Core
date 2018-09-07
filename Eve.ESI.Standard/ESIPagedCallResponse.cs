using Gware.Standard.Collections.Generic;
using System.Collections.Generic;

namespace Eve.ESI.Standard
{
    public class ESIPagedCallResponse<T> where T : ESIItemBase
    {
        public Dictionary<int, ESICollectionCallResponse<T>> Pages
        {
            get; private set;
        }

        public ESIPagedCallResponse()
        {
            Pages = new Dictionary<int, ESICollectionCallResponse<T>>();
        }

        internal void AddPage(ESICollectionCallResponse<T> page)
        {
            if (!Pages.ContainsKey(page.Page))
            {
                Pages.Add(page.Page, page);
            }
        }

        public List<T> GetAllData()
        {
            List<T> retVal = new List<T>();

            foreach(int key in Pages.Keys)
            {
                retVal.AddRange(Pages[key].Data ?? new List<T>());
            }

            return retVal;
        }

        public ESICollectionCallResponse<T> FirstPage
        {
            get
            {
                return Pages.Get(1);
            }
        }

        public bool IsSuccess
        {
            get
            {
                bool retVal = true;

                foreach(ESICollectionCallResponse<T> response in Pages.Values)
                {
                    retVal &= response.IsSuccess;
                }

                return retVal;
            }
        }
    }
}
