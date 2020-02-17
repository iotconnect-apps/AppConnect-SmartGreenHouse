using System;
using System.Dynamic;
using System.Data;
using System.Data.Common;
using iot.solution.data.Utilities;

namespace iot.solution.data
{
    public class DynamicDataReader : DynamicObject
    {
        IDataReader DataReader;
        public DynamicDataReader(IDataReader dataReader)
        {
            DataReader = dataReader;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            // 'Implement' common reader properties directly
            if (binder.Name == "IsClosed")            
                result = DataReader.IsClosed;                            
            else if (binder.Name == "RecordsAffected")            
                result = DataReader.RecordsAffected;                         
            // lookup column names as fields
            else
            {
                try
                {
                    result = DataReader[binder.Name];
                    if (result == DBNull.Value)
                        result = null;                    
                }
                catch 
                {
                    result = null;
                    return false;
                }
            }

            return true;
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
			// Implement most commonly used method
			if (binder.Name == "Read")
				result = DataReader.Read();
			else if (binder.Name == "Close")
			{
				DataReader.Close();
				result = null;
			}
			else			
                // call other DataReader methods using Reflection (slow - not recommended)
                // recommend you use full DataReader instance
                result = ReflectionUtils.CallMethod(DataReader, binder.Name, args);

            return true;            
        }
    }
}
