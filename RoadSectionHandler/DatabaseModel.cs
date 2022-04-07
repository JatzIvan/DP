using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoadSectionHandler
{
    public interface DatabaseModel<T>
    {
        public T MapReaderToObject(NpgsqlDataReader reader);

    }
}
