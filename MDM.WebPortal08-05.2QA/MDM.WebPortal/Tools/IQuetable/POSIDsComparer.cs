using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;

namespace MDM.WebPortal.Tools.IQuetable
{

    class POSIDsComparer : IEqualityComparer<VMZoomDB_POSID>
    {
        public bool Equals(VMZoomDB_POSID x, VMZoomDB_POSID y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) ||
                Object.ReferenceEquals(y, null))
                return false;

            return x.ZoomPos_ID == y.ZoomPos_ID && x.ZoomPos_Name == y.ZoomPos_Name;
        }

        public int GetHashCode(VMZoomDB_POSID posid)
        {
            if (Object.ReferenceEquals(posid, null)) return 0;

            int hashTextual = posid.ZoomPos_Name == null
                ? 0 : posid.ZoomPos_Name.GetHashCode();

            int hashDigital = posid.ZoomPos_ID.GetHashCode();

            return hashTextual ^ hashDigital;
        }
    }
}