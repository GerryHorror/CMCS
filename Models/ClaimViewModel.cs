/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

/*
    The ClaimViewModel class serves as a ViewModel in the Model-View-Controller (MVC) pattern.
    Its primary purpose is to encapsulate and organise data that will be passed from the controller to the view in the CMCS application.
    This class is specifically designed to handle data related to claims and their associated documents.
    By using this ViewModel, the application can efficiently pass complex data structures to the view,
    ensuring that the view has all the necessary information to render the claims and their associated documents.
*/

namespace CMCS.Models
{
    public class ClaimViewModel
    {
        public List<ClaimModel> Claims { get; set; }
        public Dictionary<int, List<DocumentModel>> Documents { get; set; }
    }
}