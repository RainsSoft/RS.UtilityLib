//////////////////////////////////////////////////////////////////////
// Algorithmia is (c) 2018 Solutions Design. All rights reserved.
// https://github.com/SolutionsDesign/Algorithmia
//////////////////////////////////////////////////////////////////////
// COPYRIGHTS:
// Copyright (c) 2018 Solutions Design. All rights reserved.
// 
// The Algorithmia library sourcecode and its accompanying tools, tests and support code
// are released under the following license: (BSD2)
// ----------------------------------------------------------------------
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met: 
//
// 1) Redistributions of source code must retain the above copyright notice, this list of 
//    conditions and the following disclaimer. 
// 2) Redistributions in binary form must reproduce the above copyright notice, this list of 
//    conditions and the following disclaimer in the documentation and/or other materials 
//    provided with the distribution. 
// 
// THIS SOFTWARE IS PROVIDED BY SOLUTIONS DESIGN ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL SOLUTIONS DESIGN OR CONTRIBUTORS BE LIABLE FOR 
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR 
// BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//
// The views and conclusions contained in the software and documentation are those of the authors 
// and should not be interpreted as representing official policies, either expressed or implied, 
// of Solutions Design. 
//
//////////////////////////////////////////////////////////////////////
// Contributers to the code:
//		- Frans  Bouma [FB]
//////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;

using SD.System_Linq;

using SD.Tools.Algorithmia.UtilityClasses;

namespace SD.Tools.Algorithmia.Graphs.Algorithms
{
	/// <summary>
	/// 有向图拓扑排序算法的实现。拓扑排序是一种确定哪个顶点依赖于哪些顶点的方法，因此必须首先对其进行处理。
	/// 拓扑排序依赖于深度优先搜索（Tarjan 1976：http://www.springerlink.com/content/k5633403j221763p/） 
	/// 这里的算法实现有两种方向解释选择，它由传递给构造函数的标志控制：
	/// 如果有向边 A 到 B 意味着 A 必须在 B 之前完成，则为标志 <i>directionMeansOrder</i> 传递 true，否则为 false。
	/// 默认值为 false（A 到 B 表示 A 依赖于 B，因此 B 应该在 A 之前完成）。
	/// Implementation of the Topological Sort algorithm for directed graphs. Topological sorting is a way to determine which vertex is relying on which vertices
	/// so it has to be processed first. Topological sorting relies on Depth first search (Tarjan 1976: http://www.springerlink.com/content/k5633403j221763p/)
	/// The algorithm implementation here has two choices for direction interpretation, which is controlled by a flag passed to the constructor: if the directed
	/// edge A to B means A has to be done before B, pass true for the flag <i>directionMeansOrder</i>, otherwise false. Default is false (A to B means A is depending on
	/// B, so B should be done before A).
	/// See for more information: http://en.wikipedia.org/wiki/Topological_sorting
	/// </summary>
	/// <typeparam name="TVertex">The type of the vertex.</typeparam>
	/// <typeparam name="TEdge">The type of the edge.</typeparam>
	/// <remarks>By definition topological sorting isn't possible on directed graphs which have at least one cycle (A->B->C->A->D). This algorithm will throw
	/// an InvalidOperationException exception if it detects a cycle.</remarks>
	public class TopologicalSorter<TVertex, TEdge> : DepthFirstSearchCrawler<TVertex, TEdge>
		where TEdge : class, IEdge<TVertex>
	{
#region Class Member Declarations
		private readonly bool _directionMeansOrder;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="TopologicalSorter&lt;TVertex, TEdge&gt;"/> class.
		/// </summary>
		/// <param name="graphToCrawl">要爬网的有向图。The directed graph to crawl.</param>
		/// <remarks>假设从 A 到 B 的有向边意味着 A 依赖于 B，因此 B 应放在 A 之前.
		/// Assumes a directed edge from A to B means A is depending on B and therefore B should be placed before A</remarks>
		public TopologicalSorter(GraphBase<TVertex, TEdge> graphToCrawl)
			: this(graphToCrawl, false)
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="TopologicalSorter&lt;TVertex, TEdge&gt;"/> class.
		/// </summary>
		/// <param name="graphToCrawl">The directed graph to crawl.</param>
		/// <param name="directionMeansOrder">If set to true, a directed edge from A to B is interpreted as the order in which A and B should be done, i.e. first A
		/// then B. When false is passed (default) the edge from A to B is interpreted as A is depending on B and therefore B should be done before A.</param>
		public TopologicalSorter(GraphBase<TVertex, TEdge> graphToCrawl, bool directionMeansOrder) : base(graphToCrawl, true)
		{
			ArgumentVerifier.CantBeNull(graphToCrawl, "graphToCrawl");
			ArgumentVerifier.ShouldBeTrue(g=>g.IsDirected, graphToCrawl, "graphToCrawl has to be a directed graph");
			this.SortResults = new List<TVertex>();
			this.SeeCycleCreatingEdgesAsNonExistend = false;
			_directionMeansOrder = directionMeansOrder;
		}


		/// <summary>
		/// 在传递给构造函数的图上运行算法
		/// Runs the algorithm on the graph passed to the constructor
		/// </summary>
		public void Sort()
		{
			this.Crawl();
		}


		/// <summary>
		/// 在有向图中检测到一个循环。调用此方法是为了向派生类发出已检测到循环的信号，
		/// 因为派生类命令此类在检测到循环时向其发出信号。周期检查仅在有向图上执行。
		/// A cycle has been detected in a directed graph. This method is called to signal derived classes that the cycle has been detected, as the derived class
		/// ordered this class to signal it if cycles were detected. Cycle checks are only performed on directed graphs.
		/// </summary>
		/// <param name="relatedVertex">The related vertex which formed the cycle.</param>
		/// <param name="edges">The edges used to visit the related vertex.</param>
		/// <returns>
		/// true if routines should proceed deeper into the graph, false otherwise (so traversing will stop at this point and backtrack with
		/// other paths). Returning true could lead to infinite loops. Default is true
		/// </returns>
		/// <remarks>It's recommended you throw an exception to quit the operation entirely if your algorithm can't deal with cycles.</remarks>
		protected override bool CycleDetected(TVertex relatedVertex, HashSet<TEdge> edges)
		{
			if(this.SeeCycleCreatingEdgesAsNonExistend)
			{
				// flag the base crawler code that it should break off the traversal and silently allow the cycle.
				return false;
			}
			string edgesDescription = string.Empty;
			if(edges != null)
			{
				edgesDescription = string.Join(" | ", edges.Select(e => e.ToString()).ToArray());
			}
			throw new InvalidOperationException(
				string.Format("Cycle detected. Topological sorting can't be applied on a directed graph with one or more cycles. Related vertex (which was reached from one of the vertices already visited): {0}. Edge(s) followed (and vertices visited) to reach this related vertex: {1}", 
					relatedVertex, edgesDescription));
		}


		/// <summary>
		/// 在指定的边上访问顶点时调用。在访问了与 vertexVisited 相关的所有顶点后，将立即调用此方法。
		/// Called when the vertexVisited was visited over the edges specified. This method is called right after all vertices related to vertexVisited were visited.
		/// </summary>
		/// <param name="vertexVisited">The vertex visited right before this method.</param>
		/// <param name="edges">The edges usable to visit vertexVisited. Can be null, in which case the vertex was visited without using an edge (which would mean
		/// the vertex is a tree root, or the start vertex.)</param>
		protected override void OnVisited(TVertex vertexVisited, HashSet<TEdge> edges)
		{
			if(_directionMeansOrder)
			{
				// simply insert the visited node at the start, as the calls to OnVisited are in the reverse topological order. 
				this.SortResults.Insert(0, vertexVisited);
			}
			else
			{
				// simply append the visited node at the end. 
				this.SortResults.Add(vertexVisited);
			}
		}


		#region Class Property Declarations
		/// <summary>
		/// 获取或设置排序结果。这是传递到构造函数的图的一个可能的正确拓扑顺序。拓扑排序不能保证只有 1 个排序，可以有很多正确的排序。
		/// Gets or sets the sort results. This is one possible correct topological order of the graph passed into the constructor. Topological sorting doesn't guarantee
		/// that there is just 1 ordering, there can be many correct orderings. 
		/// </summary>
		public List<TVertex> SortResults { get; private set; }
		/// <summary>
		/// 获取或设置一个值，该值指示创建循环的边是应被视为实数 （true） 还是可忽略且对结果没有影响的边（false，默认值）。
		/// “False”表示检测到周期时会引发异常。 True 表示遍历在重新访问的顶点处中断，并继续使用回溯。
		/// 将此属性保留为 false，除非应允许循环。
		/// Gets or sets a value indicating whether edges which create a cycle should be seen as real (true) or as edges which can be ignored and
		/// have no influence on the outcome (false, default). 'False' means that an exception is thrown when a cycle is detected. 
		/// True means that the traversal is broken off at the re-visited vertex and continued using backtracking. Leave this property to false, 
		/// unless cycles should be allowed.
		/// </summary>
		public bool SeeCycleCreatingEdgesAsNonExistend { get; set; }
#endregion
	}
}
