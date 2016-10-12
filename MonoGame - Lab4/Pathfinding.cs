﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame___Lab4
{
   class PathFinding
   {
      public const int OBLIQUE = 14;
      public const int STEP = 10;
      public char[,] MapArray { get; private set; }
      List<Point> CloseList;
      List<Point> OpenList;

      public PathFinding(char[,] map)
      {
         this.MapArray = map;
         OpenList = new List<Point>(MapArray.Length);
         CloseList = new List<Point>(MapArray.Length);
      }

      public Point FindPath(Point start, Point end, bool IsIgnoreCorner)
      {
         Console.WriteLine("start: (" + start.X + ", " + start.Z + ")  end: (" + end.X + ", " + end.Z + ")");
         OpenList.Add(start);
         while (OpenList.Count != 0)
         {
            //找出F值最小的点
            var tempStart = OpenList.MinPoint();
            OpenList.RemoveAt(0);
            CloseList.Add(tempStart);
            //找出它相邻的点
            var surroundPoints = SurrroundPoints(tempStart, IsIgnoreCorner);
            foreach (Point point in surroundPoints)
            {
               if (OpenList.Exists(point))
                  //计算G值, 如果比原来的大, 就什么都不做, 否则设置它的父节点为当前点,并更新G和F
                  FoundPoint(tempStart, point);
               else
                  //如果它们不在开始列表里, 就加入, 并设置父节点,并计算GHF
                  NotFoundPoint(tempStart, end, point);
            }
            if (OpenList.Get(end) != null)
               return OpenList.Get(end);
         }
         return OpenList.Get(end);
      }

      private void FoundPoint(Point tempStart, Point point)
      {
         var G = CalcG(tempStart, point);
         if (G < point.G)
         {
            point.ParentPoint = tempStart;
            point.G = G;
            point.CalcF();
         }
      }

      private void NotFoundPoint(Point tempStart, Point end, Point point)
      {
         point.ParentPoint = tempStart;
         point.G = CalcG(tempStart, point);
         point.H = CalcH(end, point);
         point.CalcF();
         OpenList.Add(point);
      }

      private int CalcG(Point start, Point point)
      {
         int G = (Math.Abs(point.X - start.X) + Math.Abs(point.Z - start.Z)) == 2 ? STEP : OBLIQUE;
         int parentG = point.ParentPoint != null ? point.ParentPoint.G : 0;
         return G + parentG;
      }

      private int CalcH(Point end, Point point)
      {
         int step = Math.Abs(point.X - end.X) + Math.Abs(point.Z - end.Z);
         return step * STEP;
      }

      //获取某个点周围可以到达的点
      public List<Point> SurrroundPoints(Point point, bool IsIgnoreCorner)
      {
         var surroundPoints = new List<Point>(9);

         for (int z = point.Z - 1; z <= point.Z + 1; z++)
            for (int x = point.X - 1; x <= point.X + 1; x++)
            {
               if (CanReach(point, x, z, IsIgnoreCorner))
                  surroundPoints.Add(x, z);
            }
         return surroundPoints;
      }

      //在二维数组对应的位置不为障碍物
      private bool CanReach(int x, int z)
      {
         return MapArray[z, x] != 'r';
      }

      public bool CanReach(Point start, int x, int z, bool IsIgnoreCorner)
      {
         if (!CanReach(x, z) || CloseList.Exists(x, z))
            return false;
         else
         {
            if (Math.Abs(x - start.X) + Math.Abs(z - start.Z) == 1)
               return true;
            //如果是斜方向移动, 判断是否 "拌脚"
            else
            {
               if (CanReach(Math.Abs(x - 1), z) && CanReach(x, Math.Abs(z - 1)))
                  return true;
               else
                  return IsIgnoreCorner;
            }
         }
      }
   }

   //Point 类型
   public class Point
   {
      public Point ParentPoint { get; set; }
      public int F { get; set; }  //F=G+H
      public int G { get; set; }
      public int H { get; set; }
      public int X { get; set; }
      public int Z { get; set; }

      public Point(int x, int z)
      {
         this.X = x;
         this.Z = z;
      }
      public void CalcF()
      {
         this.F = this.G + this.H;
      }
   }

   //对 List<Point> 的一些扩展方法
   public static class ListHelper
   {
      public static bool Exists(this List<Point> points, Point point)
      {
         foreach (Point p in points)
            if ((p.X == point.X) && (p.Z == point.Z))
               return true;
         return false;
      }

      public static bool Exists(this List<Point> points, int x, int z)
      {
         foreach (Point p in points)
            if ((p.X == x) && (p.Z == z))
               return true;
         return false;
      }

      public static Point MinPoint(this List<Point> points)
      {
         points = points.OrderBy(p => p.F).ToList();
         return points[0];
      }
      public static void Add(this List<Point> points, int x, int z)
      {
         Point point = new Point(x, z);
         points.Add(point);
      }

      public static Point Get(this List<Point> points, Point point)
      {
         foreach (Point p in points)
            if ((p.X == point.X) && (p.Z == point.Z))
               return p;
         return null;
      }

      public static void Remove(this List<Point> points, int x, int z)
      {
         foreach (Point point in points)
         {
            if (point.X == x && point.Z == z)
               points.Remove(point);
         }
      }
   }
}
