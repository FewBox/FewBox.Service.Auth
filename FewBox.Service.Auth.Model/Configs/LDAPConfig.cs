namespace FewBox.Service.Auth.Model.Configs
{
    public class LDAPConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string BatchSyncPath { get; set; }
        public string SyncPath { get; set; }
    }
}
