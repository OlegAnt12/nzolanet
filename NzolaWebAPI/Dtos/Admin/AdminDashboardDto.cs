namespace NzolaWebAPI.Dtos.Admin
{
    public class AdminDashboardDto
    {
        public int TotalUtilizadores { get; set; }
        public int TotalPublicacoes { get; set; }
        public int TotalBazes { get; set; }
        public int TotalDenuncias { get; set; }
        public int DenunciasPendentes { get; set; }
        public int UtilizadoresAtivos { get; set; }
        public int UtilizadoresPrivados { get; set; }
    }
}
