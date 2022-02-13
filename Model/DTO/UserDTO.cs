using System;

namespace Model.DTO
{
    public class UserDTO{

        public UserDTO(string fullName,string email,string userName,DateTime dataCreated)
        {
            FullName = fullName;
            Email = email;
            UserName = userName;
            DataCreated = dataCreated;
        }
        public string FullName {get; set;}
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime DataCreated { get; set; }
        public string Token { get; set; }
    }
}