﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_lite_database_search_wpf.Core.DatabaseItems.objects
{
    public class sqliteBase
    {
        public string valueName { get; set; }
        public sqliteBase(string Name) { valueName = Name; }
    }
}