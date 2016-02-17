using System;
using System.Collections.Generic;

namespace CKANMetacheck
{
    public class ModInfo
    {
        public string version;
        public string downloadUrl;
        public List<KeyValuePair<string,string>> installs = new List<KeyValuePair<string, string>>();
    }
}

