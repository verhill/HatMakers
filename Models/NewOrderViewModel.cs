namespace hatmaker_team2.Models
{
    public class NewOrderViewModel
    {
        public List<Customer> AllCustomers { get; set; }
        public Customer SelectedCustomer { get; set; }
        public Order CurrentOrder { get; set; }

        public List<Hat> StandardHats { get; set; }
        public bool CustomerConfirmed { get; set; } = false;

        public List<Hat> HatsInOrder { get; set; } = new List<Hat>();

        public List<Order_Contains_Hat> OrderContainsHats { get; set; } = new List<Order_Contains_Hat>();

        public List<Hat_Made_Of_Material> MaterialsInHat { get; set; } = new List<Hat_Made_Of_Material>();

        public List<Material> MaterialsInDb { get; set; } = new List<Material>();
    }
}