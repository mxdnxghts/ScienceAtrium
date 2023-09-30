using System;
namespace ScienceAtrium.Domain.UserAggregate
{
	public class User
	{
		public User(Guid id) : base(id)
        {
		}

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public string PhoneNumber { get; set; }
        
        public enum UserType
        {
            Customer,
            Executor
        }
    }
}

