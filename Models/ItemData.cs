using System.ComponentModel.DataAnnotations;

namespace netcore_rest_api.Models {

  public class ItemData {
    [Key]
    public int ItemID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Done { get; set; }
  }
}