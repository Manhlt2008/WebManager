using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public int RoomId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateDateStamp { get; set; }
        public bool IsRead { get; set; }
        public int Status { get; set; }
    }
}
