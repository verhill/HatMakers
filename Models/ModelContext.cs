using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace hatmaker_team2.Models
{
    public class ModelContext : IdentityDbContext<User>
    {
        public ModelContext(DbContextOptions<ModelContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hat> Hats { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<User_Manage_Hat_Orders> UserManageHatOrders { get; set; }
        public DbSet<Order_Contains_Hat> OrderContainsHats { get; set; }
        public DbSet<Hat_Made_Of_Material> HatmadeOfMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.CreatedOrders)
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.HasOrders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<User_Manage_Hat_Orders>()
                .HasKey(umh => umh.Id);

            
            modelBuilder.Entity<User_Manage_Hat_Orders>()
                .HasOne(umh => umh.User)
                .WithMany(u => u.UserManageHatsInOrder)
                .HasForeignKey(umh => umh.UserId);

            modelBuilder.Entity<User_Manage_Hat_Orders>()
                .HasOne(umh => umh.Hat)
                .WithMany(h => h.HatManagedByUser)
                .HasForeignKey(umh => umh.HatId);

            modelBuilder.Entity<User_Manage_Hat_Orders>()
                .HasOne(umh => umh.Order)
                .WithMany(o => o.UserManageHatsInOrder)
                .HasForeignKey(umh => umh.OrderId);


            modelBuilder.Entity<Order_Contains_Hat>()
                .HasKey(och => och.Id);


            modelBuilder.Entity<Order_Contains_Hat>()
                .HasOne(och => och.Order)
                .WithMany(o => o.HatsInOrder)
                .HasForeignKey(och => och.OrderId);

            modelBuilder.Entity<Order_Contains_Hat>()
                .HasOne(och => och.Hat)
                .WithMany(h => h.HatInOrder)
                .HasForeignKey(och => och.HatId);

            
            modelBuilder.Entity<Hat_Made_Of_Material>()
                .HasKey(hmm => new { hmm.HatId, hmm.MaterialId });

            
            modelBuilder.Entity<Hat_Made_Of_Material>()
                .HasOne(hmm => hmm.Hat)
                .WithMany(h => h.MaterialUsedInHat)
                .HasForeignKey(hmm => hmm.HatId);

            modelBuilder.Entity<Hat_Made_Of_Material>()
                .HasOne(hmm => hmm.Material)
                .WithMany(m => m.MaterialInHat)
                .HasForeignKey(hmm => hmm.MaterialId);

            

            //User user1 = new User
            //{
            //    Id = "user1-id",
            //    Firstname = "Otto",
            //    Lastname = "Hattmakarsson",
            //    UserName = "otto@hatmail.com",
            //    NormalizedUserName = "OTTO@HATMAIL.COM",
            //    Email = "otto@hatmail.com",
            //    NormalizedEmail = "OTTO@HATMAIL.COM",
            //    EmailConfirmed = true,
            //    IsAdmin = true,
            //    SecurityStamp = "static-stamp-1",
            //    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
            //};
            

            //User user2 = new User
            //{
            //    Id = "user2-id",
            //    Firstname = "Judith",
            //    Lastname = "Hattsson",
            //    UserName = "judith@hatmail.com",
            //    NormalizedUserName = "JUDITH@HATMAIL.COM",
            //    Email = "judith@hatmail.com",
            //    NormalizedEmail = "JUDITH@HATMAIL.COM",
            //    EmailConfirmed = true,
            //    IsAdmin = true,
            //    SecurityStamp = "static-stamp-2",
            //    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
            //};
            

            //User user3 = new User
            //{
            //    Id = "user3-id",
            //    Firstname = "Tanja",
            //    Lastname = "Havstorm",
            //    UserName = "tanja@hatmail.com",
            //    NormalizedUserName = "TANJA@HATMAIL.COM",
            //    Email = "tanja@hatmail.com",
            //    NormalizedEmail = "TANJA@HATMAIL.COM",
            //    EmailConfirmed = true,
            //    IsAdmin = true,
            //    SecurityStamp = "static-stamp-3",
            //    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
            //};
            

            //User user4 = new User
            //{
            //    Id = "user4-id",
            //    Firstname = "Andreas",
            //    Lastname = "Ask",
            //    UserName = "andreas@hatmail.com",
            //    NormalizedUserName = "ANDREAS@HATMAIL.COM",
            //    Email = "andreas@hatmail.com",
            //    NormalizedEmail = "ANDREAS@HATMAIL.COM",
            //    EmailConfirmed = true,
            //    IsAdmin = true,
            //    SecurityStamp = "static-stamp-4",
            //    PasswordHash = "AQAAAAIAAYagAAAAEB9kc022lIacHYyCoxijpmN+44SOYcC7aU9dT8mrGfLCZhD/iaJVzaxrWrWGklfeMA=="
            //};
            

            //modelBuilder.Entity<User>().HasData(user1, user2, user3, user4);
        }
    }
}
