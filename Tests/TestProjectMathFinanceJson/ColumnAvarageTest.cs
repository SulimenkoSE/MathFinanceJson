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
        //������� ����� ������ ��� �������� � �����. ����� ������� �������������� ���������
        public static IEnumerable<object[]> mathDbData
        {
            get
            {
                yield return new object[]{ new List<MathDbKript>
                    {
                    new MathDbKript( 0, "2013-02-01 00:00:00", 21.10),
                    new MathDbKript( 1, "2013-02-02 00:00:00", 21.20),
                    new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
                    new MathDbKript( 3, "2013-02-04 00:00:00", 21.30),
                    new MathDbKript( 4, "2013-02-05 00:00:00", 22.30),
                    new MathDbKript( 5, "2013-02-06 00:00:00", 23.40)
                    },
                    21.86, 5, 400, FieldsMathDbKript.AdjClose, 0 //��������� ��������� result
                };

                yield return new object[]{ new List<MathDbKript>
               {
                new MathDbKript( 0, "2013-02-01 00:00:00", 21.10),
                new MathDbKript( 1, "2013-02-02 00:00:00", 21.10),
                new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
                new MathDbKript( 3, "2013-02-04 00:00:00", 21.10),
                new MathDbKript( 4, "2013-02-05 00:00:00", 21.10)
                },
                21.10, 5, 400, FieldsMathDbKript.AdjClose, 0 //��������� ��������� result => double result, int col_end, int period, FieldsMathDbKript fieldsTable, int col_start 
                };
            }
        }
        
        public static IEnumerable<object[]> mathDbData_NEW(int col_end, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            yield return new object[]{ new List<MathDbKript>
                    {
                        new MathDbKript( 0, "2013-02-01 00:00:00", 21.10),
                        new MathDbKript( 1, "2013-02-02 00:00:00", 21.20),
                        new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
                        new MathDbKript( 3, "2013-02-04 00:00:00", 21.30),
                        new MathDbKript( 4, "2013-02-05 00:00:00", 22.30),
                        new MathDbKript( 5, "2013-02-06 00:00:00", 23.40)
                    },
                        21.86, col_end, period, fieldsTable, col_start //��������� ��������� result
                };

            yield return new object[]{ new List<MathDbKript>
                   {
                        new MathDbKript( 0, "2013-02-01 00:00:00", 21.10),
                        new MathDbKript( 1, "2013-02-02 00:00:00", 21.10),
                        new MathDbKript( 2, "2013-02-03 00:00:00", double.NaN),
                        new MathDbKript( 3, "2013-02-04 00:00:00", 21.10),
                        new MathDbKript( 4, "2013-02-05 00:00:00", 21.10)
                    },
                        21.10, col_end, period, fieldsTable, col_start //��������� ��������� result
                    };
        }

        //�������� �������� �� public static IEnumerable<object[]> mathDbData
        //https://xunit.net/faq/theory-data-stability-in-vs
        //���� �� ��������� �� ������������� Test Explorer, �� ������ ��������� xUnit.net,
        //��� ���� ������ �����������, ������ ��� �� ��������� ������������ ������ �� ����� �����������
        //[MemberData(nameof(mathDbData), DisableDiscoveryEnumeration = true)]

        [Trait("Avarage", "NaNa")]
        //[InlineAutoData(5, 400, FieldsMathDbKript.AdjClose, 0)]
        //[InlineAutoData(3, 200, FieldsMathDbKript.AdjClose, 1)]
        [Theory]
        [MemberData(nameof(mathDbData))]
        public void Avarage_VenEmpty_Return_NaNa(List<MathDbKript> data, double result, int col_end, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            var sut = new ColumnAverage();
            var newResult = sut.Avarage(data, period, col_end, fieldsTable);
            //����������� ��e�� ������� result
            newResult.Should().Be(result);
            Assert.Equal(result, newResult);
        }

        [Trait("Avarage", "NaN")]
        [InlineAutoData(0)]
        [InlineAutoData(1000)]
        [Theory]
        public void Avarage_VenEmpty_Return_NaN(int col_end, MathDbKript element, int period, FieldsMathDbKript fieldsTable, int col_start)
        {
            //������� ������ ����� ������
            List<MathDbKript> data = new() { element };
            var sut = new ColumnAverage();
            var result = sut.Avarage(data, period, col_end, fieldsTable);
            //����������� ���� ������� double.NaN
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

    }
}