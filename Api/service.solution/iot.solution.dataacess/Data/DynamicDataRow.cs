using System;
using System.Dynamic;
using System.Data;

namespace iot.solution.data
{
    public class DynamicDataRow : DynamicObject
    {
        DataRow DataRow;
        public DynamicDataRow(DataRow dataRow)
        {
            DataRow = dataRow;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            try
            {
                result = DataRow[binder.Name];

                if (result == DBNull.Value)
                    result = null;
                
                return true;
            }
            catch { }

            result = null;
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                if (value == null)
                    value = DBNull.Value;

                DataRow[binder.Name] = value;
                return true;
            }
            catch {}

            return false;
        }
    }
}
