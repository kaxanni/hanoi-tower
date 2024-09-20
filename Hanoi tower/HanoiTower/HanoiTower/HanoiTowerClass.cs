using System.Collections.Generic;
using System.Linq;

namespace HanoiTower
{
	public class HanoiTowerClass
	{
		private int steps = 0;

		Stack<int> Tower1 = new Stack<int>();
		Stack<int> Tower2 = new Stack<int>();
		Stack<int> Tower3 = new Stack<int>();

		private void Reset()
		{
			Tower1.Clear();
			Tower2.Clear();
			Tower3.Clear();
		}

		private Stack<int> GetTower(Towers tower)
		{
			if (tower == Towers.Left)
				return Tower1;
			else if (tower == Towers.Middle)
				return Tower2;
			else
				return Tower3;
		}

		public List<int> GetTowerLeft()
		{
			return Tower1.ToList<int>();
		}

		public List<int> GetTowerMiddle()
		{
			return Tower2.ToList<int>();
		}

		public List<int> GetTowerRight()
		{
			return Tower3.ToList<int>();
		}

		public void Init(int steps)
		{
			Reset();
			this.steps = steps > Constant.MaxSteps ? Constant.MaxSteps : steps;
			for (int i = this.steps; i > 0; i--)
				Tower1.Push(i);
		}

		public int GetStep(Towers from)
		{
			Stack<int> fromTower = GetTower(from);
			if (fromTower.Count < 1)
				return 0;
			return fromTower.Peek();
		}

		public bool Move(Towers from, Towers to)
		{
			int fromStep = 0;
			int toStep = 0;

			Stack<int> fromTower = GetTower(from); ;
			Stack<int> toTower = GetTower(to); ;

			if (fromTower.Count < 1) return false;

			fromStep = fromTower.Peek();

			if (toTower.Count < 1) toStep = 10;
			else toStep = toTower.Peek();

			if (fromStep < toStep)
			{
				toTower.Push(fromTower.Pop());
				return true;
			}
			else if (fromStep == toStep)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool IsComplete()
		{
			return Tower1.Count == 0 && (Tower2.Count == steps || Tower3.Count == steps);
		}
	}

	public enum Towers
	{
		Left,
		Middle,
		Right
	}
}
