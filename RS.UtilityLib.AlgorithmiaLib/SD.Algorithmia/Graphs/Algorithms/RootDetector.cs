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
//		- Walaa Atef [WA]
//////////////////////////////////////////////////////////////////////
using System;
#if NET_2_0
using SD.System_Linq;
#else
using System.Collections.Generic;
#endif


namespace SD.Tools.Algorithmia.Graphs.Algorithms
{
	/// <summary>
	/// DepthFirstSearchCrawler实现用于检测非有向图中的根（断开连接的子图）的数量，这有助于识别断开连接的图。
	/// DepthFirstSearchCrawler implementation to detect the number of roots ( disconnected subgraphs) within a non-directed graph, which helps identifing a 
	/// disconnected graph.        
	/// </summary>
	/// <typeparam name="TVertex">Type of the vertex of the graph to crawl</typeparam>
	/// <typeparam name="TEdge">Type of the edge of the graph to crawl</typeparam>
	/// <remarks>图形必须是无向的。有向图可以提供比实际更多的根，这就是在有向图上使用此算法会导致 InvalidOperationException 的原因。
	/// Graph has to be undirected. Directed graphs could give more roots than there really are, which is the reason that using this algorithm on a
	/// directed graph results in an InvalidOperationException.</remarks>
	public class RootDetector<TVertex, TEdge> : DepthFirstSearchCrawler<TVertex, TEdge>, IRootDetector
		where TEdge : class, IEdge<TVertex>
	{
#region Class Member Declarations
		private readonly HashSet<TVertex> _rootsFound;
#endregion

		/// <summary>
		/// CTor
		/// </summary>
		/// <param name="toCrawl"></param>
		public RootDetector(GraphBase<TVertex, TEdge> toCrawl)
			: base(toCrawl, false)
		{
			if(toCrawl.IsDirected)
			{
				throw new InvalidOperationException("This algorithm can't be used on a directed graph");
			} 
			_rootsFound = new HashSet<TVertex>();
		}


		/// <summary>
		/// 在图形中搜索根
		/// Searches for roots within the graph
		/// </summary>
		/// <returns>number of roots detected, 1 signifies a connected graph, more than 1 signifies a disconnected graph</returns>
		public int SearchForRoots()
		{
			_rootsFound.Clear();
			this.Crawl();
			return _rootsFound.Count;
		}


		/// <summary>
		/// 发出信号，检测爬网程序已访问的根顶点。
		/// Signal the detection of a root vertex that has been visited by the crawler.
		/// </summary>
		/// <param name="vertexVisited">The detected root vertex</param>
		/// <remarks>Only called in non-directed graphs, as root detection isn't possible with a DFS crawler in directed graphs without additional algorithm code</remarks>
		protected override void RootDetected(TVertex vertexVisited)
		{
			_rootsFound.Add(vertexVisited);
		}


		#region Class Property Declarations
		/// <summary>
		///找到根。 Gets the roots found.
		/// </summary>
		public HashSet<TVertex> RootsFound
		{
			get { return _rootsFound; }
		}
#endregion
	}
}
