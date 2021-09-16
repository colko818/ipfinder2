using System.Collections.Generic;

namespace Manager.Models {

    public class TaskData {
        public string addr;
        public string cmd;
    }

    public class IpFinderRequest {
        public string Address { get; set; }
        public List<string> Commands { get; set; }
    }
}