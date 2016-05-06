using System;
using System.Collections.Generic;
using System.Linq;

namespace LendingTree.Web.Mvc
{
    public interface IWeighted
    {
        double Weight { get; }
    }

    public class WeightedRandomization
    {
        static Random random = new Random();
        public static T Choose<T>(IEnumerable<T> list) where T : IWeighted
        {
            if (!list.Any())
                return default(T);

            var rand = random.NextDouble();
            foreach (var item in list)
            {
                if (rand < item.Weight)
                    return item;

                rand -= item.Weight;
            }

            throw new InvalidOperationException("The proportions in the collection do not add up to 1.");
        }

        public static T ChooseOld<T>(IEnumerable<T> list) where T : IWeighted
        {
            if (!list.Any())
                return default(T);

            double totalweight = list.Sum(c => c.Weight);
            Random rand = new Random();
            double choice = rand.NextDouble();
            double sum = 0;

            foreach (var listItem in list)
            {
                for (double i = sum; i < listItem.Weight + sum; i++)
                {
                    if (i >= choice)
                    {
                        return listItem;
                    }
                }
                sum += listItem.Weight;
            }

            return list.First();
        }
    }
}
