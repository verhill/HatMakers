namespace hatmaker_team2.Models
{
    public class User_Order_ViewModel
    {
        public List<Order> AllOrders {  get; set; } 
        public List<User> AllUsers { get; set; }
        public List<User_Manage_Hat_Orders> AllUsersManage { get; set; }
        public string CurrentUserId { get; set; }
    }
}
