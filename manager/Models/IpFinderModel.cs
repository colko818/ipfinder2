using System.Collections.Generic;

namespace Manager.Models {

    // Struct wrapper to send address & command to a newly spawned worker
    public class TaskData {
        public string addr { get; set; }
        public string cmd { get; set; }
    }

    // Defines public API request data
    public class IpFinderRequest {
        public string Address { get; set; }
        public List<string> Commands { get; set; }
    }
}