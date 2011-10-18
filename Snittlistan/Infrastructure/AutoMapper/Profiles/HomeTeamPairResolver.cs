using System.Collections.Generic;

namespace Snittlistan.Infrastructure.AutoMapper.Profiles
{
	/// <summary>
	/// H1-B1 H2-B2 H3-B3 H4-B4
	/// H3-B4 H4-B3 H1-B2 H2-B1
	/// H4-B2 H3-B1 H2-B4 H1-B3
	/// H2-B3 H1-B4 H4-B1 H3-B2.
	/// </summary>
	public class HomeTeamPairResolver : PairResolver
	{
		private static Dictionary<int, int[]> scheme = new Dictionary<int, int[]>
		{
			{ 0, new int[] { 0, 2, 3, 1 } },
			{ 1, new int[] { 1, 3, 2, 0 } },
			{ 2, new int[] { 2, 0, 1, 3 } },
			{ 3, new int[] { 3, 1, 0, 2 } },
		};

		protected override int[] GetScheme(int pair)
		{
			return scheme[pair];
		}
	}
}