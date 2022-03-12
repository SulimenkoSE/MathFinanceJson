using Xunit;
using MathJson;
using System;
using System.Collections.Generic;
using System.Collections;    
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using NSubstitute;

namespace TestProjectMathFinanceJson
{
    public class ColumnAvarageTest
    {
        //Создаем набор данных для передачи в тесты. После запятой предполагаемый результат
        //public static IEnumerable<object[]> mathDbData
        //{
        //    get
        //    {
        //        yield return new object[]{ new List<MathDbKript>
        //            {
        //            new MathDbKript( 0, "2013-02-01 00:00:00", 21.10),
        //            new MathDbKript( 1, "2013-02-02 00:00:00", 21.20),
        //            new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
        //            new MathDbKript( 3, "2013-02-04 00:00:00", 21.30),
        //            new MathDbKript( 4, "2013-02-05 00:00:00", 22.30),
        //            new MathDbKript( 5, "2013-02-06 00:00:00", 23.40)
        //            },
        //            21.86, 5, 400, FieldsMathDbKript.AdjClose, 0 //ожидаемый результат result
        //        };

        //        yield return new object[]{ new List<MathDbKript>
        //       {
        //        new MathDbKript( 0, "2013-02-01 00:00:00", 21.10),
        //        new MathDbKript( 1, "2013-02-02 00:00:00", 21.10),
        //        new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
        //        new MathDbKript( 3, "2013-02-04 00:00:00", 21.10),
        //        new MathDbKript( 4, "2013-02-05 00:00:00", 21.10)
        //        },
        //        21.10, 5, 400, FieldsMathDbKript.AdjClose, 0 //ожидаемый результат result => double result, int col_end, int period, FieldsMathDbKript fieldsTable, int col_start 
        //        };
        //    }
        //}

        public static IEnumerable<object[]> mathDbData(int col_end, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            yield return new object[]{ new List<MathDbKript>
                    {
                        new MathDbKript( 0, "2013-02-01 00:00:00", 21.10),
                        new MathDbKript( 1, "2013-02-02 00:00:00", 20.20),
                        new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
                        new MathDbKript( 3, "2013-02-04 00:00:00", 22.30),
                        new MathDbKript( 4, "2013-02-05 00:00:00", 22.30),
                        new MathDbKript( 5, "2013-02-06 00:00:00", 23.40)
                    },
                        21.86, col_end, period, fieldsTable, col_start //ожидаемый результат result 21,859999999999996
                };

            yield return new object[]{ new List<MathDbKript>
                   {
                        new MathDbKript( 0, "2013-02-01 00:00:00", 21.10){Return = 12.235 },
                        new MathDbKript( 1, "2013-02-02 00:00:00", -21.10),
                        new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
                        new MathDbKript( 3, "2013-02-04 00:00:00", 21.10),
                        new MathDbKript( 4, "2013-02-05 00:00:00", 21.10),
                        new MathDbKript( 5, "2013-02-06 00:00:00", -21.10)
                    },
                        4.22, col_end, period, fieldsTable, col_start //ожидаемый результат result 4.220000000000001
                    };
        }

        //получаем значение из public static IEnumerable<object[]> mathDbData
        //https://xunit.net/faq/theory-data-stability-in-vs
        //Если вы настроены на использование Test Explorer, вы можете намекнуть xUnit.net,
        //что ваши данные нестабильны, сказав ему не выполнять перечисление данных во время обнаружения
        //[MemberData(nameof(mathDbData), DisableDiscoveryEnumeration = true)]

        [Trait("Avarage", "NaNa")]
        //[InlineAutoData(5, 400, FieldsMathDbKript.AdjClose, 0)]
        //[InlineAutoData(3, 200, FieldsMathDbKript.AdjClose, 1)]
        [Theory]
        [MemberData(nameof(mathDbData), 5, 400, FieldsMathDbKript.AdjClose, 0)]
        public void Avarage_returns_correct_result(List<MathDbKript> data, double result, int col_end, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            var sut = new ColumnAverage();
            var newResult = sut.Avarage(data, period, col_end, fieldsTable);
            //результатом емeть возврат result
            newResult.Should().Be(result);
            Assert.Equal(result, newResult);
        }

        [Trait("Avarage", "NaN")]
        [InlineAutoData(0)]
        [InlineAutoData(1000)]
        [Theory]
        public void Avarage_VenEmpty_Return_NaN(int col_end, MathDbKript element, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            //создаем пустой набор данных
            List<MathDbKript> data = new() { element };
            var sut = new ColumnAverage();
            var result = sut.Avarage(data, period, col_end, fieldsTable);
            //результатом еcть возврат double.NaN
            result.Should().Be(double.NaN);

        }

        [Trait("Avarage", "Throws")]
        [AutoData]
        [Theory]
        public void Avarage_VenEmpty_Throws(int period, int col_end, FieldsMathDbKript fieldsTable, int col_start)
        {
            List<MathDbKript> data = new();
            var sut = new ColumnAverage();
            Func<double> GetMimoRange = () => sut.Avarage(data, period, col_end, fieldsTable);
            GetMimoRange.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*ven*");
        }

        [Trait("btcIssuance", "Throws")]
        [Fact]
        public void btcIssuance_Ven_Index_1_492_992__Throws()
        {
            //Создаем новый рассчетный файл данных
            List<MathDbKript> data = new List<MathDbKript>
            {
                new MathDbKript( 1024*1458, "2013-02-01 00:00:00", 21.10)
            };

            var sut = new ColumnAverage();
            Func<double> GetMimoRange = () => sut.btcIssuance(data, 0);
            //результатом еcть возврат double.NaN
            GetMimoRange.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*ven*");
        }

        [Trait("btcIssuance", "Correct")]
        [InlineAutoData(0, 7200)]
        [InlineAutoData(1458, 3600)]
        [Theory]
        public void btcIssuance_returns_correct_result(int index, double results)
        {
            //Создаем новый рассчетный файл данных
            List<MathDbKript> data = new List<MathDbKript>
            {
                new MathDbKript( index, "2013-02-01 00:00:00", 21.10)
            };

            var sut = new ColumnAverage();
            var result = sut.btcIssuance(data, 0);
            //результатом еcть возврат results
            result.Should().BeApproximately(results, 3);
        }

        [Trait("ReturnPersant", "NaN")]
        [InlineAutoData(0)]
        [Theory]
        public void ReturnPersant_VenEmpty_Return_NaN(int col_end, MathDbKript element)
        {
            //создаем пустой набор данных
            List<MathDbKript> data = new() { element };
            var sut = new ColumnAverage();
            var result = sut.ReturnPersant(data, col_end);
            //результатом еcть возврат double.NaN
            result.Should().Be(double.NaN);
        }

        [Trait("ReturnPersant", "Throws")]
        [InlineAutoData(1)]
        [InlineAutoData(100)]
        [Theory]
        public void ReturnPersant_VenEmpty_Throws(int col_end)
        {
            List<MathDbKript> data = new();
            var sut = new ColumnAverage();
            Func<double> GetMimoRange = () => sut.ReturnPersant(data, col_end);
            GetMimoRange.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*ven*");
        }

        [Trait("ReturnPersant", "Correct")]
        [Theory]
        [MemberData(nameof(mathDbData), 4, 400, FieldsMathDbKript.AdjClose, 0)]
        public void ReturnPersant_Correct_Result(List<MathDbKript> data, double results, int col_end, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            var sut = new ColumnAverage();
            var result = sut.ReturnPersant(data, col_end);
            result.Should().Be(0);
        }


        [Trait("Return_365_STD", "NaN")]
        [InlineAutoData(365, 0, FieldsMathDbKript.Return, 0)]
        [InlineAutoData(365, 5, FieldsMathDbKript.Return, 5)]
        [Theory]
        public void Return_365_STD_Return_NaN(int period, int col_end, FieldsMathDbKript fieldsTable, int col_start)
        {
            List<MathDbKript> data = new();
            var sut = new ColumnAverage();
            var result = sut.Return_365_STD(data, period, col_end, fieldsTable, col_start); //
            result.Should().Be(double.NaN);
        }

        [Trait("Return_365_STD", "Throws")]
        [InlineAutoData(1, 365, 0)]
        [InlineAutoData(6, 365, 0)]
        [Theory]
        public void Return_365_STD_VenEmpty_Throws(int col_end, int period,  int col_start)
        {
            List<MathDbKript> data = new();
            var sut = new ColumnAverage();
            Func<double> GetMimoRange = () => sut.Return_365_STD(data, period, col_end, FieldsMathDbKript.Return, col_start);
            GetMimoRange.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*ven*");
        }

        [Trait("Return_365_STD", "Correct")]
        [Theory]
        [MemberData(nameof(mathDbData), 5, 365, FieldsMathDbKript.AdjClose, 0)]
        public void Return_365_STD(List<MathDbKript> data, double results, int col_end, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            //Подключаем клас для проведения расчета по даннім полученнім из файла            
            var sut = new ColumnAverage();   
            var result = sut.Return_365_STD(data, period, col_end, fieldsTable, col_start);
            if (results > 6) result.Should().Be(1.10381157812373);
            if (results <= 6) result.Should().Be(20.673693429090022);
        }


    }
}