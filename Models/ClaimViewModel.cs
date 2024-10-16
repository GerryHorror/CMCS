namespace CMCS.Models
{
    public class ClaimViewModel
    {
        public List<ClaimModel> Claims { get; set; }
        public Dictionary<int, List<DocumentModel>> Documents { get; set; }
    }
}