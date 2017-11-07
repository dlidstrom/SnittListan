using System;

namespace Snittlistan.Web.Areas.V2.Domain
{
    public class Player
    {
        public Player(string name, string email, Status playerStatus, int personalNumber, string nickname)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            Email = email;
            PlayerStatus = playerStatus;
            PersonalNumber = personalNumber;
            Nickname = nickname ?? name;
        }

        public enum Status
        {
            Active,
            Supporter,
            Inactive
        }

        public string Id { get; set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public Status PlayerStatus { get; private set; }

        public int PersonalNumber { get; private set; }

        public string Nickname { get; private set; }

        public void SetName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        public void SetStatus(Status status)
        {
            PlayerStatus = status;
        }

        public void SetPersonalNumber(int personalNumber)
        {
            PersonalNumber = personalNumber;
        }

        public void SetNickname(string nickname)
        {
            Nickname = nickname ?? Name;
        }
    }
}