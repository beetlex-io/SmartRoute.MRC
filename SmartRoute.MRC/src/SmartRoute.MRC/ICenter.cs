using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoute.MRC
{
	public interface ICenter : IDisposable
	{

		String ID { get; }

		INode Node { get; set; }


		IUserService UserService { get; set; }

		void Open();

	}
}
