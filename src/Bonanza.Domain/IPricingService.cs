using System;
using System.Collections.Generic;
using System.Text;

namespace Bonanza.Domain
{
	public interface IPricingService
	{
		decimal GetOverdraftThreshold();
		decimal GetWelcomeBonus();
	}


	public sealed class PricingService : IPricingService
	{
		public decimal GetOverdraftThreshold()
		{
			return 1;
		}

		public decimal GetWelcomeBonus()
		{
			return 2;
		}
	}
}
