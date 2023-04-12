﻿//////////////////////////////////////////////////////////////////////
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


namespace SD.Tools.Algorithmia.Graphs
{
	/// <summary>
	/// 与图中的边类一起使用的接口。
	/// Interface to be used with Edge classes in graphs.
	/// </summary>
	public interface IEdge<TVertex>
	{
		/// <summary>
		/// 取边的起始顶点。Gets the start vertex of the edge.
		/// </summary>
		TVertex StartVertex { get;}
		/// <summary>
		///获取边的结束顶点。 Gets the end vertex of the edge. 
		/// </summary>
		TVertex EndVertex { get; }
		/// <summary>
		/// 获取一个值，该值指示此边是否有向。 如果为真，则边从 startVertex 指向 endVertex，
		/// 并且仅被视为 startVertex 和 endVertex 之间的边，而不是 endVertex 和 startVertex 之间的边。
		/// 如果为 false，则此边不被视为有向边，而是被视为 startVertex 和 endVertex 之间
		/// 以及 endVertex 和 startVertex 之间的边。
		/// Gets a value indicating whether this edge is directed. If true, the edge is directed from startVertex to endVertex and is seen as an edge only between
		/// startVertex and endVertex, not between endVertex and startVertex. If false, this edge isn't considered a directed edge and is seen as an edge between 
		/// startVertex and endVertex and also between endVertex and startVertex.
		/// </summary>
		bool IsDirected { get; }
	}
}
