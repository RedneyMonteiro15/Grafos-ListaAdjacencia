using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;

namespace P3Proj16Grafos
{
	/// <summary>
	/// Classe para simular estrutura de dados do tipo grafo
	/// Também aqui deveriamos usar generics para tornar a nossa classe mais abrangente
	/// no entanto, para simplificar o estudo vamos usar:
	/// - string para identificar os diferentes vertícies:
	/// - int para as ligações (apenas valores positivos; 0 ou <0 para inexistente...)
	/// </summary>
	public class Grafo
	{
     // lista de cores... para desenhar grafo
     static KnownColor[] colorNames  = (KnownColor[])Enum.GetValues(typeof(KnownColor));

     private int totalVertices=0;        // guarda número de vétices do grafo
	 public string[] vertices=null;      // lista de nomes dos vértices do Grafo
	 //dicionario para guardar todas as
	 // informações das ligações
	 //uso do dicionario, pois permitir guardar qual é o vertice(key) e qual a distancia(value)
	 public Dictionary<int, int>[] ramos;    //diciomnario
	 public Point[] posicoes=null;		// guarda as coordenadas para representação gráfica
	
		
#region Construtor da Classe
		public Grafo(int NumeroDeVertices)
		{
			this.vertices = new string[NumeroDeVertices]; // espaço para nomes dos vertices
			this.posicoes = new Point[NumeroDeVertices];
			this.totalVertices=NumeroDeVertices;
			Random random = new Random();
			ramos = new Dictionary<int, int>[NumeroDeVertices];
			for (int i=0;i<this.totalVertices;i++)
			{
			 	this.posicoes[i]= new Point(random.Next(10,700),random.Next(10,500));	
				this.vertices[i]="Vertice " + i;
				ramos[i] = new Dictionary<int, int>();
			}
		}
#endregion
		
#region Propriedades da Classe
    // devolve o número de vétices que constituem o Grafo
	public int TotalVertices
	{
		get 
		{
			return this.totalVertices;
		}
	}

	// Propriedade que calcula e devolve o número de ramos válidos no grafo
	// lembrar existe ligação de valor na matriz >0
	// Ordem de Complexidade Temporal: 
	public int TotalRamos
	{
		get 
		{
			int c = 0;
			for(int i = 0;  i < ramos.Length; i++)
			{
				c += ramos[i].Count;
			}
			return c;
		}
	}
	
	// propriedade que devolve a densidade do Grafo: R/(V*(V-1))
	public double Densidade
	{
		get 
		{
		  return (double)this.TotalRamos/(this.totalVertices*(this.totalVertices-1));
		}		
	}
	
#endregion		

#region Métodos Públicos dos objectos da Classe
   // devolve o Grafo formatado em forma de matriz de adjacência
   // para colocar numa textBox qualquer...
   // cada linha tem 13 espaços + 5*numvertices + 2 caracteres (mudar de linha)
   // (não mexer!!!)
	public override string ToString()
	{
		 string saida="             ";
	   	 // primera linha  .... 00  01  20  03
	   	 for (int i=0;i<this.totalVertices;i++)
	   	 	saida=saida+String.Format(": {0:00} ",i);
	   	 saida = saida + "\r\n";    // mudar de linha
	   	 // linhas seguintes:
	   	 for (int i=0;i<this.totalVertices;i++)
	   	 {
	   	 	saida = saida + String.Format("{0,-11}{1:00}",(this.vertices[i].Length>11)?this.vertices[i].Substring(0,11):this.vertices[i].ToString(),i);
	   	  	for (int j=0;j<this.totalVertices;j++)
	   	  	{
	   	  		if (this.ramos[i].ContainsKey(j) == false) //se o j não estiver no dicionario não é para mostrar (dá erro), isto é, se não é sucessor 
		   	  	   saida=saida+String.Format(":{0,3} ","".Substring(0));
		   	    else //se o j esta dentro do dicionario, é sucessor do j, mostra o mesmo
		   	    	saida=saida+String.Format(":{0,4}",(this.ramos[i][j]<=0)?"":this.ramos[i][j].ToString());
	   	  	}
	   	  saida = saida + "\r\n";
   	  	}
   	 	return saida.Substring(0,saida.Length-2); // devolver tirando o último \r\n
	}
	
	public Image ToImage(int comprimento, int altura)
	{
	 if (comprimento<=0 || altura<=0)
			return null;
		 
	 Pen pen1 = new Pen(Color.DarkSalmon,2);		 
	 Font font = new System.Drawing.Font("Currier New", 9,FontStyle.Regular, GraphicsUnit.Point);
	 StringFormat f = new StringFormat();
	 f.Alignment= StringAlignment.Center;

	 Image imag = new Bitmap(comprimento,altura,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
	 Graphics g=Graphics.FromImage(imag);
	 g.Clear(Color.LightGray);
	 if (this.vertices==null)
	 	  return imag; 		 	 
	 AdjustableArrowCap bigArrow = new AdjustableArrowCap(4, 8);	 		 
	 // desenhar os ramos
     Pen pen2 = new Pen(Color.DarkRed,2); 
     pen2.CustomEndCap = bigArrow;
	 for (int i=0;i<this.totalVertices;i++)
	      {
	 	   int px=this.posicoes[i].X;
	 	   int py=this.posicoes[i].Y;
	 	   pen2.Color=Color.FromKnownColor(colorNames[(i*3)%colorNames.Length]);
	 	   for (int j=0;j<this.totalVertices;j++)
	 	   	if (this.ramos[i].ContainsKey(j))
	        {
	 	   	g.DrawLine(pen2,px,py,this.posicoes[j].X,this.posicoes[j].Y);
	       	g.DrawString(this.ramos[i][j].ToString(),font,Brushes.Blue,(px+this.posicoes[j].X)/2+1,(py+this.posicoes[j].Y)/2-5,f);
	           }
		  }

	 // Agora desenhar, "por cima", os vertíces
	 for (int i=0;i<this.totalVertices;i++)
	      {
	 	   g.DrawArc(pen1,this.posicoes[i].X-10,this.posicoes[i].Y-10,20,20,0,360);
	 	   g.DrawString(this.vertices[i]+"("+i+")",font,Brushes.Blue,this.posicoes[i].X,this.posicoes[i].Y-10,f);
		  }

	 g.Dispose();
	 return imag; 		 
	}
  	
   // método que desenha numa imagem o trajeto contido no vertor vert
   public Image DrawInImage(int[] vert, Image img)
   {
   	if (vert==null || img==null || this.totalVertices==0)
   		return null;
		 
	 Pen pen1 = new Pen(Color.Yellow,4);		 
     pen1.EndCap = LineCap.ArrowAnchor;
	 Graphics g=Graphics.FromImage(img);
	 // desenhar as ligações
	 for (int i=0;i<vert.Length-1;i++)
	      {
	 	   g.DrawLine(pen1,this.posicoes[vert[i]].X,this.posicoes[vert[i]].Y,this.posicoes[vert[i+1]].X,this.posicoes[vert[i+1]].Y);
		  }
	 g.Dispose();
	 return img; 		 
	}
   
   // método que faz a gravação dos dados de um grafo em disco
   // usando um formato próprio....
   // 1ª linha: texto GrafoP3
   // 2ª linha: total de vertices (N)
   // N linhas: nomes dos vertices
   // N linhas: matriz de ligação (valores separados por espaços e com * onde não há ligações)
   public void ToFile(string nomeF)
   {
   	StreamWriter fs = new StreamWriter(nomeF,false,System.Text.Encoding.GetEncoding("iso-8859-1"));
   	fs.WriteLine("GrafoP3");
   	fs.WriteLine(this.totalVertices.ToString());
   	// colocar no ficheiro o nome dos vertices e suas coordenadas...
   	for (int i=0;i<this.totalVertices;i++)
   		fs.WriteLine("{0};{1};{2}",this.vertices[i],this.posicoes[i].X,this.posicoes[i].Y);
   	// colocar no ficheiro os dados das ligações
   	for (int i=0;i<this.totalVertices;i++)
    {
   		for (int j=0;j<this.totalVertices;j++) //utilização do for porque precisamos percorrer todos os vertices, não só os sucessores do vertices atual(utilização do foreach)
			if (!this.ramos[i].ContainsKey(j)) //se não conter o verice j no dicionario, escreve *
			{
				fs.Write("* ");
			}
			else
			{
				fs.Write("{0} ",this.ramos[i][j].ToString());   //escreve a distancia do vertice no ficheiro
			}
     	fs.WriteLine();
	}
   	fs.Close();
   }

   // método que calcula e devolve o grau de um vértice
   // faz uso de outros métodos para o calculo...
   public int GrauVertice(int vert)   //basta saber quantos sucessores e quantos antecessores
   {
   	return TotalAntecessores(vert)+TotalSucessores(vert);
   }
   
   /// <summary>
   /// método que devolve um vector com os índices dos vértices sucessores de Vini
   /// </summary>
   /// <param name="vini">Indice do vertice inicial onde começa a listagem</param>
   /// <returns>vector contendo os indices dos vertices</returns>
   public int[] Sucessores(int vini)
   {
   		int[] vetor = new int[ramos[vini].Count]; //neste se sabe qual é a quantidade de vertices que será guardado (o total de vertices)
   		int i = 0;
   		foreach(KeyValuePair<int, int> item in ramos[vini])
   		{
   			vetor[i] = item.Key;
   			i++;
   		}
   		return vetor;
   }   
   /// <summary>
   /// método que devolve um vector com os índices dos vértices Antecessores de Vini
   /// </summary>
   /// <param name="vini">Indice do vertice inicial onde começa a listagem</param>
   /// <returns>vector contendo os indices dos vertices</returns>
   public int[] Antecessores(int vini)
   {
   		List<int> lst = new List<int>();  //como não sei apriori a dimensão do vetor, decidi usar uma lista
   		for(int j = 0; j < ramos.Length; j++)	//percorre todos os vertices
   		{
   			if(ramos[j].ContainsKey(vini)) //se o vini for sucessor do j, adiciona na lista porque é antecessor de vini
		    {
   				lst.Add(j);
		    }
   		}
   		return lst.ToArray();
   }
   
   /// <summary>
   /// método que permite obter a lista de nomes de vertices (na forma de string), separados por ;
   /// </summary>
   /// <param name="vecindices">vetor de índices a saber o nome</param>
   /// <returns>string contendo a lista de nomes sparada por ;</returns>
   public string IndicesToNomes(int[] vecIndices)
   {
   	string saida =" ";
   	for (int i = 0; i < vecIndices.Length; i++)
   	{
   		saida += this.vertices[vecIndices[i]] + " ; ";
   	}
   	return saida;
   }
   
   /// <summary>
   /// Implementação da travessia do grafo 
   /// usando como estratégia a pesquisa em Largura (BreadthFirst)
   //  devolve um vector contendo os vértices visitados ao longo dessa travessia
   /// </summary>
   /// <param name="vi">Indice do vertice onde começa a travessia</param>
   /// <returns>lista (vector) de vertices atravessados por esta travessia</returns>
   public int[] BreadthFirst(int vi)
   {
   	if (vi<0 || vi>=this.totalVertices)
   		return null;
   	bool[] visitados = new bool[this.totalVertices];// para marcar visitados
   	List<int> lista = new List<int>();		  // para colocar travessia..
   	Queue<int> fila = new Queue<int>();   // guardar temporaiamente os valores
   	fila.Enqueue(vi);		// coloca na fila o 1º vertice
	visitados[vi]=true;		// marca-o como visitado
	while(fila.Count>0)
	{
		int vx = fila.Dequeue();
		lista.Add(vx);
		//acrescentar todos os sucessores de vx, não visitados à fila (marcando-os também como processados
		foreach(KeyValuePair<int, int> item in ramos[vx])  //usei o foreach para percorrer todos os sucessoresde vx
		{
			if(visitados[item.Key] == false) //se não foi visitado
			{
				fila.Enqueue(item.Key);
				visitados[item.Key] = true;
			}
		}
	}
	return lista.ToArray();
   }
   
   /// <summary>
   /// Implementação da travessia do grafo 
   /// usando como estratégia a pesquisa em Profundidade (usa recursividade)
   //  devolve um vector contendo os vértices visitados ao longo dessa travessia
   /// </summary>
   /// <param name="vi">Indice do vertice onde começa a travessia</param>
   /// <returns>lista (vector) de vertices atravessados por esta travessia</returns>
   public int[] DepthFirst(int vi)
   {
   	if (vi<0 || vi>=this.totalVertices)
   		return null;
   	bool[] visitados= new bool[this.totalVertices];
   	List<int> lista = new List<int>();
   	DepthFirstRecursivo(vi,visitados,lista);
   	
   	return lista.ToArray();
   }
   
   // método que verifica se existe um qualquer caminho entre dois vértices do grafo
   // devolve true em caso afirmativo.
   public bool ExisteCaminho(int vini, int vfim)
   {
	    bool[] visitados = new bool[this.totalVertices];   
	   	return Ha_Caminho(vini,vfim,visitados);
   }

   // método que verifica a existencia de um caminho entre dois vértices 
   // devolvendo-o na forma de vector (null caso não exista)
   public int[] UmCaminho(int vini, int vfim)
   {   	
   	bool[] visitados = new bool[this.totalVertices];
   	List<int> lista = new List<int>();
   
   	if (Ha_Caminho(vini,vfim,visitados,lista)==true)
   		lista.Add(vini);
   	lista.Reverse();
   	return lista.ToArray();
   }
   
   /// <summary>
   /// método que calcula qual o menor caminho entre dois vétices
   /// implementa o algoritmo de dijkstra
   /// </summary>
   /// <param name="vini">Indice do vertice partida</param>
   /// <param name="vfim">Indice do vertice destino</param>
   /// <returns>devolve o caminho a percorrer na forma de vector (null caso não exista caminho possível)</returns>
   public int[] MenorCaminho(int vini, int vfim)
	{
		bool[] visitado = new bool[this.totalVertices]; 
		int[] distancia = new int[this.totalVertices];
		int[] anterior = new int[this.totalVertices]; //para poder saber o caminho "de volta"   	
		// iniciar vetores
		//for (int i=0;i<this.totalVertices;i++)
		for (int i=0;i<this.totalVertices;i++)
		{
			visitado[i]=false;
		    anterior[i]=vini;
		    if(ramos[vini].ContainsKey(i)) //se o i for um sucessor do vini, adiciona a distancia entre os dois
	        {
		    	distancia[i] = ramos[vini][i];
	        }
		    else //senão não existe ligação entre eles, a distancia não se sabe fica com uma dicancia de 100000			
		    {
		    	distancia[i]=100000;
		    }
		   	
		}
		// algoritmo propriamente dito...
		distancia[vini]=0;
		visitado[vini]=true;
		int vx;
		while((vx = IndMenorDistanciaNVisitado(distancia, visitado))!=-1)
		{
			visitado[vx] = true;
			for(int  j = 0; j< this.totalVertices; j++)
	   		{
				if(this.ramos[vx].ContainsKey(j) && visitado[j] == false) //se j for sucessor e j ainda não for visitado
	   			{
					if(distancia[j]>distancia[vx]+this.ramos[vx][j]) 
	   				{
						distancia[j] = distancia[vx] + this.ramos[vx][j];
	   					anterior[j] = vx;
	   				}
	   			}
	   		}
		}
		// no fim do while temos em anterior o caminho mais curto para todos os vert.
		// vamos agora reconstruir o caminho de vini para vfim
		if (distancia[vfim]==100000)
			return null;				// não existe caminho
		
		Stack<int> caminho = new Stack<int>();
		caminho.Push(vfim);
		int ant = vfim;
		while(anterior[ant] != vini)
		{
			ant = anterior[ant];
			caminho.Push(ant);
		}
		caminho.Push(vini);
		return caminho.ToArray();
	}
   
   /// <summary>
   /// método que transforma o grafo actual num grafo de menores distâncias de todos para todos
   /// faz uso do algortimo de Floyd-Warshall para este objectivo
   /// </summary>
   public void ToGrafoMenoresDistancias()
   {
		int[,] dist = new int[this.totalVertices,this.totalVertices];
		//iniciar distancias
		for (int i=0;i<this.totalVertices;i++)
		{
			for (int j=0;j<this.totalVertices;j++)
			{
			    if(ramos[i].ContainsKey(j)) //se o i for um sucessor do vini, adiciona a distancia entre os dois
		        {
			    	dist[i, j] = ramos[i][j];
		        }
			    else //senão não existe ligação entre eles, a distancia não se sabe fica com uma dicancia de 100000			
			    {
			    	dist[i, j] = 100000;
			    }
			}
		}
   }
   
   // método que transforma o grafo actual numa árvore geradora minima
   // faz uso do algortimo de Prim para este objectivo
   public void ToArvoreGeradoraMinima(int vini)
   {
	   	bool[] visitado = new bool[this.totalVertices]; 
	   	int[] distancia = new int[this.totalVertices];
	   	int[] anterior = new int[this.totalVertices]; //para poder reconstruir o grafo
	   	for (int i=0;i<this.totalVertices;i++)
   	   {
   		distancia[i]=100000;
   	   }
	   	anterior[vini]=-1;
	   	distancia[vini]=0;
   }
   
   /// <summary>
   /// Método que calcula e devolve a lista de vertinices inacessíveis
   /// a partir de um vertice inical.
   /// </summary>
   /// <param name="vini">vértice inicial</param>
   /// <returns>lista contendo indices inacessíves. Null caso não exista</returns>
   public int[] Inacessiveis(int vini)
   {
   		List<int> lista = new List<int>(BreadthFirst(vini));
   		lista.Sort();
   		List<int> lst = new List<int>();
   		for(int i = 0; i < totalVertices; i++)
   		{
   			if(lista.IndexOf(i) == -1)
   			{
   				lst.Add(i);
   			}
   		}
   		return lst.ToArray();
   }
   #endregion

#region Métodos privados da classe
    // Método auxiliar da travessia em Profundidade...
    private void DepthFirstRecursivo(int vi,bool[] visit, List<int> lst)
    {
	     visit[vi]=true;		// marca-lo como visitado
	     lst.Add(vi);			// acrescenta-lo à saída..
	     // Na lista de sucessores (não visitados) voltar a chamar este mesmo
	     // método de forma recursiva...
	     foreach(KeyValuePair<int, int> item in ramos[vi])//percorre todos os sucessores de vi
		 {
			if(visit[item.Key] == false) //se não estiver visitado chama o ketodo recursivo de DepthFirstRecursivo
			{
				DepthFirstRecursivo(item.Key, visit, lst);
			}
		 }
    }
    

	/// <summary>
    /// Método auxiliar para verificar se existe caminho entre dois vértices..
    /// Faz uso da definição de caminho...
	/// </summary>
	/// <param name="vini">indice do vertice inicial</param>
	/// <param name="vfim">indice do vertice final</param>
	/// <param name="visit">boolenos para assinalar já visitados</param>
	/// <returns></returns>
    private bool Ha_Caminho(int vini, int vfim,bool[] visit)
    {
     visit[vini]=true;
     if(this.ramos[vini].ContainsKey(vfim)) //se for
     {
     	return true;
     }
     // existe caminho de vini para vfim se houver ramo vini->vfim	
     foreach(KeyValuePair<int, int> item in ramos[vini])  //percorre todos os sucessores de vini
	{
		if(visit[item.Key] == false)
     	{
     		bool result = Ha_Caminho(item.Key, vfim, visit);
     		if(result == true)
     		{
     			return true;
     		}
     	}
	}
     return false;	// por aqui não há caminho...
    }    
    
    
	/// <summary>
    /// Método auxiliar para verificar se existe caminho entre dois vértices (
    /// caso exista coloca esse caminho numa lista ligada
    /// Faz uso da definição de caminho...
	/// </summary>
	/// <param name="vini">indice do vertice inicial</param>
	/// <param name="vfim">indice do vertice final</param>
	/// <param name="visit">boolenos para assinalar já visitados</param>
	/// <param name="lista">lista ligada contendo caminho </param>
	/// <returns></returns>
    private bool Ha_Caminho(int vini, int vfim,bool[] visit, List<int> lista)
    {
     visit[vini]=true;
     // existe caminho de vini para vfim se houver ramo vini->vfim	
     if(this.ramos[vini].ContainsKey(vfim)) //se for sucessor
     {
     	lista.Add(vfim);
     	return true;
     }
  
     foreach(KeyValuePair<int, int> item in ramos[vini])//percorre todo os sucessores de vini
	 {
		if(visit[item.Key] == false) //se não for visitadi
     	{
     		bool result = Ha_Caminho(item.Key, vfim, visit, lista);
     		if(result == true)
     		{
     			lista.Add(item.Key);
     			return true;
     		}
     	}
	}
     return false;	
    }
      
	// método que devolve o número de sucessores de um vértice
	private int TotalSucessores(int vini)
	{
		return this.ramos[vini].Count; //retorna o total de sucessos, chama a função Count para saber os sucessores
	}

	// método que devolve o número de Antecessores de um vértice
	private int TotalAntecessores(int vini)
	{
	   int c=0;
	   for(int i = 0; i < totalVertices; i++)//percorre todos os vertices do grafo
		{
	   		if(this.ramos[i].ContainsKey(vini)) //se vini for sucessor de i, é porque é antecessor de vini 
			{
				c++; //incrementa mais um em c
			}
		}
	 	return c;
	}

	// método que devolve o indice da menor distância (>0) de entre os vértices não visitados
	// devolve -1 caso já esteja tudo visitado
	private static int IndMenorDistanciaNVisitado(int[] dist, bool[] visit)
	{
		int ind=-1;
	 	int menor=100000;
	 	for (int i=0;i<dist.Length;i++)
	 	{
	 		if (dist[i]<menor && visit[i]==false)
        	{
 				menor=dist[i];
 	     		ind=i;
        	}
	 	}
	 	return ind;	
	}
#endregion

#region Métodos static da classe Grafo 
    // método que cria um grafo com os dados guardados num ficheiro 
    // formatado de formaespecial (1-GtafoP3, 2-numvertices, 3-nomes, 4-ramos....)
    public static Grafo GrafoFromFile(string nomeF)
    {Grafo grafo=null;
     string[] linhas;
     linhas=File.ReadAllLines(nomeF,System.Text.Encoding.GetEncoding("iso-8859-1")); //ler todos os dados do ficheiro (linha a linha)
     if (linhas.Length<6)				// Não deve ser um grafo (falta mostra mensagem...)
	 	      return null;					
     if (linhas[0]!="GrafoP3")  		// Ver 1ª linha: Não deve ser um grafo nosso (falta mostra mensagem...)
     	 return null;			
     int numvertices;
     int.TryParse(linhas[1],out numvertices);
     if (numvertices<2)
     	 return null;
     grafo = new Grafo(numvertices);   // reservar espaço para este grafo
     // ler as informações dos seus vertices..
     for (int i=0;i<numvertices;i++)
      {string[] umalinha = linhas[i+2].Split(';');
     	 if (umalinha.Length<3)
     	        return null;	
       grafo.vertices[i]=umalinha[0];
       int px,py;
       int.TryParse(umalinha[1],out px);grafo.posicoes[i].X=px;
       int.TryParse(umalinha[2],out py);grafo.posicoes[i].Y=py;
      }
     // tentar agora ler informções sobre as ligações entre vertices
     for (int i=0;i<numvertices;i++)
       {
     	string[] umalinha = linhas[numvertices+i+2].Split(' ');
     	 if (umalinha.Length<numvertices)
     	 {
     	 	return null;
     	 }
     	 for (int j=0;j<numvertices;j++) //percorre todos os vertices
     	 {
     	 	if(umalinha[j] != "*") //se estiver ligação, estiver distancia
     	 	{
     	 		grafo.ramos[i].Add(j, Convert.ToInt32(umalinha[j])); //adiciona o sucessor ao i
     	 	}
     	 }
     	 
        }     	
	 return grafo;
    }
#endregion
	}
}
