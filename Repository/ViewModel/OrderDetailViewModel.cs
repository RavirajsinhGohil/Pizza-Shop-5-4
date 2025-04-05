namespace Repository.ViewModel;

public class OrderDetailViewModel
{
    public OrdersViewModel order { get; set; }
    public CustomerViewModel customer { get; set; }
    public TableViewModel table { get; set; }
    // public List<TableViewModel> tables { get; set; }
    public SectionNameViewModel section { get; set; }
    public List<ItemViewModel> items { get; set; }
    public List<ModifiersViewModel> modifiers { get; set; }
}
