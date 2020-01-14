using System;
using System.Collections.Generic;
using System.Data;

namespace Korzh.DbUtils
{
    /// <summary>
    /// Represents one column in database table.
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// Gets the column name.
        /// </summary>
        /// <value>The name of the column.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Determines whether the column is a primary key.
        /// </summary>
        /// <value>The name of the column.</value>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// Gets the type of the column (as CLR type).
        /// </summary>
        /// <value>The type of the column data.</value>
        public Type DataType {get; private set;}

        /// <summary>
        /// Gets or sets a value indicating whether this column is a timestamp field.
        /// </summary>
        /// <value><c>true</c> if this column is a timestamp; otherwise, <c>false</c>.</value>
        public bool IsTimestamp { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="type">The name of the CLR type for the new column.</param>
        /// <param name="isPK">Is primary key.</param>
        public ColumnInfo(string name, string type, bool isPK = false) : this(name, Type.GetType(type), isPK)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the new column.</param>
        /// <param name="type">The CLR type of the new column.</param>
        /// <param name="isPK">Is primary key.</param>
        public ColumnInfo(string name, Type type, bool isPK = false)
        {
            Name = name;
            DataType = type;
            IsPrimaryKey = isPK;
        }

    }


    /// <summary>
    /// Represents one dataset (table) in some DB
    /// </summary>
    public class DatasetInfo
    {
        /// <summary>
        /// Gets the table name.
        /// </summary>
        /// <value>The name of the dataset (table).</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <value>The schema name.</value>
        public string Schema { get; private set; }

        /// <summary>
        /// Sets the schema
        /// </summary>
        /// <param name="schema"></param>
        public void SetSchema(string schema)
        {
            Schema = schema;
        }

        /// <summary>
        /// Gets the directioary which contains all table columns. 
        /// The key of each entry is the name of the column.
        /// The value - is an instance of <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <value>The list of columns.</value>
        public IReadOnlyDictionary<string, ColumnInfo> Columns => _columns;

        private Dictionary<string, ColumnInfo> _columns = new Dictionary<string, ColumnInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the dataset (table).</param>
        /// <param name="schema">The table schema.</param>
        public DatasetInfo(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        /// <summary>
        /// Adds a new column to the dataset.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(ColumnInfo column)
        {
            _columns[column.Name] = column;
        }
    }
}
