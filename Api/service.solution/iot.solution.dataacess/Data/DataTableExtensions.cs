using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Dynamic;

namespace iot.solution.data
{
    public static class DataTableDynamicExtensions
    {
        public static dynamic DynamicRow(this DataTable dt, int index)
        {
            var row = dt.Rows[index];            
            return new DynamicDataRow(row);
        }
        public static DynamicDataRows DynamicRows(this DataTable dt)
        {
            DynamicDataRows drows = new DynamicDataRows(dt.Rows);
            return drows;
        }
    }

    public class DynamicDataRows : IEnumerator<DynamicDataRow>, IEnumerable<DynamicDataRow>
    {
        DataRowCollection Rows;
        IEnumerator RowsEnumerator;

        public DynamicDataRow this[int index]
        {
            get
            {
                return new DynamicDataRow(Rows[index]);
            }
        }
        DynamicDataRow IEnumerator<DynamicDataRow>.Current
        {
            get
            {
                return new DynamicDataRow(RowsEnumerator.Current as DataRow);
            }
        }
        public object Current
        {
            get
            {
                return new DynamicDataRow(RowsEnumerator.Current as DataRow);
            }
        }
        public DynamicDataRows(DataRowCollection rows)
        {
            Rows = rows;
            RowsEnumerator = rows.GetEnumerator();
        }
        IEnumerator<DynamicDataRow> IEnumerable<DynamicDataRow>.GetEnumerator()
        {
           foreach (DataRow row in Rows)
            yield return new DynamicDataRow(row);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (DataRow row in Rows)
                yield return new DynamicDataRow(row);
        }
        public void Dispose()
        {
            Rows = null;
            RowsEnumerator = null;
        }
        public bool MoveNext()
        {
            return RowsEnumerator.MoveNext();
        }
        public void Reset()
        {
            RowsEnumerator.Reset();
        }
    }
}
