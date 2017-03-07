using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC
{
	public class MCRFactory
	{
		private static ICenter mCenter = null;
		public static ICenter Center
		{
			get
			{
				if (mCenter == null)
					mCenter = new Implement.Center(new Implement.UserService(), null);
				return mCenter;
			}
		}

		private static IGateway mGateway = null;
		public static IGateway Gateway
		{
			get
			{
				if (mGateway == null)
					mGateway = new Implement.Gateway();
				return mGateway;
			}
		}
	}
}
