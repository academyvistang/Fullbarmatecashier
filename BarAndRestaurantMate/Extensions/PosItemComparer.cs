using HotelMateWeb.Dal.DataCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarAndRestaurantMate.Extensions
{
    public class PosItemComparer : IEqualityComparer<POSItem>
    {
        public bool Equals(POSItem x, POSItem y)
        {
            return x.StockItem.Id == y.StockItem.Id;
        }

        public int GetHashCode(POSItem obj)
        {
            return obj.StockItem.Id.GetHashCode();
        }
    }

    public class RoomTypeComparer : IEqualityComparer<RoomType>
    {
        public bool Equals(RoomType x, RoomType y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(RoomType obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public class GuestOrderComparer : IEqualityComparer<GuestOrder>
    {
        public bool Equals(GuestOrder x, GuestOrder y)
        {
            return x.BarTable.TableId == y.BarTable.TableId;
        }

        public int GetHashCode(GuestOrder obj)
        {
            return obj.BarTable.TableId.GetHashCode();
        }
    }
}