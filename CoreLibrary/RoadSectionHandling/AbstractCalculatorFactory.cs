using CoreLibrary.RoadSectionHandling.CollisionCalculators;
using CoreLibrary.RoadSectionHandling.Model;
using CoreLibrary.RoadSectionHandling.RoadParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoreLibrary.RoadSectionHandling
{

    /**
     * To correctly implement automatic loading for new factory you need define loading in static context
     * Example:
     * FactoryImplementation()
     *   {
     *      GetInstance();
     *   }
     * There is a possibility to create parametrized lambdas. Sadly, parametrized lambdas are not currectly loaded automatically.
     * "Automatic" initialization needs to be implemented in the respective factories
     */

    // I: Template for calculators (generic approach)
    // A: Attribute to look for
    public abstract class AbstractCalculatorFactory<I, A> where A: AbstractCalculatorAttribute
    {
        private List<ValueTuple<Type, Func<object>>> loadedCalculators = new List<ValueTuple<Type, Func<object>>>();
        private ValueTuple<Type, Func<object>> defaultConstructor;

        protected AbstractCalculatorFactory(A attribute)
        {
            InitializeInstances(attribute);
        }

        protected static List<Type> LoadImplementations(AbstractCalculatorAttribute attr, Type assignabeType)
        {

            //Type calculatorType = typeof(ICollisionCalculatorImplementation);

            // Some reflection magic to read all "calculators" from current namespace
            // TODO test this with functional implementation
            Console.WriteLine("Loading Implementations for " + assignabeType.Name);

            List<Type> collisionImplementations = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.GetCustomAttributes<AbstractCalculatorAttribute>()
                        .Any(y => y.Type == attr.Type)).Where(p => assignabeType.IsAssignableFrom(p)).ToList();
            
            Console.WriteLine("Found and loaded " + collisionImplementations.Count + " implementations");

            foreach(Type impl in collisionImplementations)
            {
                Console.WriteLine(impl.Name);
            }
            Console.WriteLine("");
            // Some more magic to create instances of gathered types (we do not need to provide any data to constructors)
            //implKlazzes.ForEach(klazz => curveCollisionImplementations.Add((ICollisionCalculatorImplementation)Activator.CreateInstance(klazz)));
            return collisionImplementations;
        }

        /**
         * Method used to precompile type for faster creation
         * Precompile method with one input parameter
         */
        protected static Func<TArg, object> CreateCreator<TArg>(Type constructorType)
        {
            /*ConstructorInfo constructor = constructorType.GetConstructor(constructorParams);

            ParameterExpression[] expr = new ParameterExpression[constructorParams.Length];

            for (int i= 0; i< constructorParams.Length; i++)
            {
                expr[i] = Expression.Parameter(constructorParams[i], i + "");
            }

            Expression<Func<object[], object>> creatorExpression = Expression.Lambda<Func<object[], object>>(
                            Expression.New(constructor, expr), expr);
            return creatorExpression.Compile();*/

            var constructor = constructorType.GetConstructor(new Type[] { typeof(TArg) });
            var parameter = Expression.Parameter(typeof(TArg), "p");
            var creatorExpression = Expression.Lambda<Func<TArg, object>>(
                Expression.New(constructor, new Expression[] { parameter }), parameter);
            return creatorExpression.Compile();

        }

        /**
         * Method used to precompile type for faster creation
         */
        protected static Func<object> CreateCreator(Type constructorType)
        {

            var constructor = constructorType.GetConstructor(new Type[] { });
            var creatorExpression = Expression.Lambda<Func<object>>(
                Expression.New(constructor));
            return creatorExpression.Compile();

        }

        public virtual List<Type> GetLoadedTypes()
        {
            return loadedCalculators.Select(_ => _.Item1).ToList();
        }

        public abstract ValueTuple<Type, Func<object>> GetDefaultInstance();

        // Load instances of calculator based on attribute
        // Prepare and store compiled lambda for later invocations
        // Creating compiled lambda is quite slow, so only create once on startup
        public virtual void InitializeInstances(A attribute)
        {
            List<Type> calculators = LoadImplementations(attribute, typeof(I));
            foreach (Type calculator in calculators)
            {
                loadedCalculators.Add((calculator, CreateCreator(calculator)));
            }

            defaultConstructor = GetDefaultInstance();
        }

        // Create new implementation of compiled lambda
        // If provided type is not found, use default implementation (to make sure, that application will still function with incorrect setup)
        public virtual I GetResolverImplementation(string type)
        {

            ValueTuple<Type, Func<object>> foundType = loadedCalculators.Where(i => i.Item1.Name.Equals(type))
                .FirstOrDefault();

            if (foundType.Equals(default(ValueTuple<Type, Func<object>>)))
            {
                foundType = defaultConstructor;
            }

            return (I) foundType.Item2();

        }
    }
}
