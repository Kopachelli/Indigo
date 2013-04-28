﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IndigoEngine;
using IndigoEngine.Agents;
using NLog;

namespace GraphicalUI
{
    public partial class GrapgicalUIForm : Form
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();		

		#region Static methods
			
			/// <summary>
			/// Setting double buffering for any control
			/// </summary>
			/// <param name="c">Control to set double buffering</param>
			public static void SetDoubleBuffered(System.Windows.Forms.Control c)
			{
				if (System.Windows.Forms.SystemInformation.TerminalServerSession)
				{
					return;
				}
					System.Reflection.PropertyInfo aProp =
						  typeof(System.Windows.Forms.Control).GetProperty(
								"DoubleBuffered",
								System.Reflection.BindingFlags.NonPublic |
								System.Reflection.BindingFlags.Instance);

				aProp.SetValue(c, true, null);
			}

		#endregion
		
        private int textureSize = GraphicalUI.Properties.Resources.grass64.Width;  //Side of the texture (default)

        private Point shiftVector;       //shift from basic point (0, 0) to drag the map and to understand which area must be drawn
        private Point mouseDownPoint;    //Point where we start dragging the map
        private bool leftMouseButtonInMapIsPressed = false;  //Flag for dragging the map (if true - DRAG NOW!)
        private int zoomModifyer = 0;    //Zoom modifyer (contains delta adding to the texture size)
        
		private List<Agent> displayInfoAgents = new List<Agent>();       //Info about agents that must be displayed in the mapInfoPanel
		private List<Agent> currentAgentsInTheCell = new List<Agent>();  //Agents that are in the mouse coords now (necessary for choosing between agents in context menu) 

		#region Textures dictionary

			private static Dictionary<Type, Image> texturesDict = new Dictionary<Type, Image>()
			{
				{
					typeof(AgentLivingIndigo),
					GraphicalUI.Properties.Resources.indigo_suit64
				},			
				{
					typeof(AgentItemFoodFruit),
					GraphicalUI.Properties.Resources.fruit64
				},		
				{
					typeof(AgentManMadeShelterCamp),
					GraphicalUI.Properties.Resources.camp64
				},	
				{
					typeof(AgentItemResLog),
					GraphicalUI.Properties.Resources.log64
				},
				{
					typeof(AgentPuddle),
					GraphicalUI.Properties.Resources.water64
				},
				{
					typeof(AgentTree),
					GraphicalUI.Properties.Resources.tree64
				},
			}; 

		#endregion

		#region Constructors

			public GrapgicalUIForm()
			{
				InitializeComponent();

				//Setting coordinates center to the panel center
				shiftVector = new Point(- mapPanel.Width / 2, - mapPanel.Height / 2);

				//Model tick linking
				GraphicalUIShell.Model.ModelTick += new EventHandler(onModelTick);

				//Setting double buffering
				SetDoubleBuffered(this.mapPanel);
				SetDoubleBuffered(this.mapInfoPanel);
			
				// Initialize the Label and TextBox controls.
				Label label = new Label();
				label.Location = new Point(1, 1);
				label.Text = "";
				label.AutoSize = true;
				mapInfoPanel.Controls.Add(label);
			}

		#endregion

		#region Form events

			private void GrapgicalUIForm_FormClosing(object sender, FormClosingEventArgs e)
			{		
				GraphicalUIShell.Model.Stop();
			}

		#endregion

		#region Model tick events
			
			/// <summary>
			/// The model tick event itself
			/// </summary>
			private void onModelTick(object sender, EventArgs e)	
			{            
				CrossthreadRefreshMapInfoPanel();
				CrossthreadRefreshMapPanel();            
			}

			/// <summary>
			/// Crossthread map refreshing
			/// </summary>
			private void CrossthreadRefreshMapPanel()
			{
				if (mapPanel.InvokeRequired)
				{
					mapPanel.Invoke(new MethodInvoker(CrossthreadRefreshMapPanel));
					return;
				}

				mapPanel.Refresh();
			}
			
			/// <summary>
			/// Crossthread info refreshing
			/// </summary>
			private void CrossthreadRefreshMapInfoPanel()
			{
				if (mapInfoPanel.InvokeRequired)
				{
					mapInfoPanel.Invoke(new MethodInvoker(CrossthreadRefreshMapInfoPanel));
					return;
				} 

				mapInfoPanel.Refresh();  
			}

		#endregion

		#region Coordinates recalculating

			/// <summary>
			/// Calculates the UI coords from the model coords
			/// </summary>
			/// <param name="modelCoord">Coordinates in the hole world (basic point is the world center)</param>
			/// <returns>Coordinates in the panel (basic point is the left top corner)</returns>
			private Point GetUICoord(Point modelCoord)
			{
				return new Point(modelCoord.X * (textureSize + zoomModifyer) - shiftVector.X,
								-modelCoord.Y * (textureSize + zoomModifyer) - shiftVector.Y);
			}

			/// <summary>
			/// Calculates the model coordinates from UI coordinates
			/// </summary>
			/// <param name="UICoord">Coordinates in the panel (basic point is the left top corner)</param>
			/// <returns>Coordinates in the hole world (basic point is the world center)</returns>
			private Point GetModelCoord(Point UICoord)
			{
				double y = -(double)(UICoord.Y + shiftVector.Y) / (double)(textureSize + zoomModifyer);
				double x =  (double)(UICoord.X + shiftVector.X) / (double)(textureSize + zoomModifyer);
				x = x < 0 ? x-1 : x;
				y = y < 0 ? y : y+1;
				return new Point((int)x, (int)y);
			}

		#endregion

		#region Panels refreshing/redrawing

			/// <summary>
			/// Event for refreshing the map panel
			/// </summary>
			private void mapPanel_Paint(object sender, PaintEventArgs e)
			{				
				labelModelTick.Text = "ModelTick = " + GraphicalUIShell.Model.ModelIterationTick.ToString();
				
				drawMap(e);
			}

			/// <summary>
			/// Event for refreshing the info panel
			/// </summary>
			private void mapInfoPanel_Paint(object sender, PaintEventArgs e)
			{        
				mapInfoPanel.Controls[0].Text = "";
				foreach (Agent ag in displayInfoAgents)
				{
					mapInfoPanel.Controls[0].Text += ag.ToString() + "\n\n";
				}   
			}

			/// <summary>
			/// Draws the map in the given region
			/// </summary>
			private void drawMap(PaintEventArgs e)
			{
				//Current texture size to draw (considering zoom)
				int currentTextureSize = textureSize + zoomModifyer;
				//Image to draw
				Image drawedImage = GraphicalUI.Properties.Resources.grass64;
				//Point to draw in
				Point drawBegin = new Point(-currentTextureSize, -currentTextureSize);
				Point drawEnd = new Point(mapPanel.Width + currentTextureSize, mapPanel.Height + currentTextureSize);

				//Draws the background
				for (int i = drawBegin.X; i < drawEnd.X; i += currentTextureSize)
				{
					for (int j = drawBegin.Y; j < drawEnd.Y; j += currentTextureSize)
					{
						e.Graphics.DrawImage(drawedImage, i - shiftVector.X % currentTextureSize, j - shiftVector.Y % currentTextureSize, currentTextureSize, currentTextureSize);
					}
				}
				
				lock(GraphicalUIShell.Model.Agents) //Locking Agents for other threads
				{
					//Draws agents
					foreach (Agent agent in GraphicalUIShell.Model.Agents)
					{
						if (!agent.CurrentLocation.HasOwner)
						{
							texturesDict.TryGetValue(agent.GetType(), out drawedImage);
							if(drawedImage == null)
							{
								logger.Error("Failed to find texture for {0}", agent.GetType());
							}
							if (agent.GetType() == typeof(AgentTree) && agent.Inventory.ExistsAgentByType(typeof(AgentItemFoodFruit)))
							{
								drawedImage = GraphicalUI.Properties.Resources.fruit_tree64;
							}
							Point agentUICoord = GetUICoord(agent.CurrentLocation.Coords);

							if ((agentUICoord.X > drawBegin.X) && (agentUICoord.X < drawEnd.X) &&
								(agentUICoord.Y > drawBegin.Y) && (agentUICoord.Y < drawEnd.Y))
							{
								e.Graphics.DrawImage(drawedImage, agentUICoord.X, agentUICoord.Y, currentTextureSize, currentTextureSize);
							}
						}
					}
				}
				foreach (Agent agent in displayInfoAgents)
				{
					e.Graphics.DrawRectangle(new Pen(Brushes.DarkGreen), new Rectangle(GetUICoord(agent.CurrentLocation.Coords), new Size(currentTextureSize, currentTextureSize)));
				}
			}

		#endregion		

		#region Wheel event for zoom

			private void mapPanel_MouseWheel(object sender, MouseEventArgs e)
			{
				if (e.Delta > 0)
				{
					zoomModifyer += GraphicalUI.Properties.Resources.grass64.Width / 4;
				}
				if (e.Delta < 0 && zoomModifyer >= -GraphicalUI.Properties.Resources.grass64.Width / 2)
				{
					zoomModifyer -= GraphicalUI.Properties.Resources.grass64.Width / 4;
				}
				mapPanel.Refresh();
			}
			
			/// <summary>
			/// Shaytan for zoom to work
			/// </summary>
			private void mapPanel_MouseEnter(object sender, EventArgs e)
			{	
				mapPanel.Select();
			}

		#endregion

		#region Main mouse events

			private void mapPanel_MouseDown(object sender, MouseEventArgs e)
			{
				if (e.Button == MouseButtons.Left)
				{
					mouseDownPoint.X = e.X + shiftVector.X;
					mouseDownPoint.Y = e.Y + shiftVector.Y;
					leftMouseButtonInMapIsPressed = true;
				}
			}

			private void mapPanel_MouseUp(object sender, MouseEventArgs e)
			{
				if (e.Button == MouseButtons.Left)
				{
					leftMouseButtonInMapIsPressed = false;
				}
				if (e.Button == MouseButtons.Right)
				{					
					SelectAgentAt(e.Location);
				}
			}

			private void mapPanel_MouseMove(object sender, MouseEventArgs e)
			{			
				labelUICoords.Text = "UI coords: " + e.Location.ToString();
				labelModelCoords.Text = "Model coords: " + GetModelCoord(e.Location).ToString();

				if (leftMouseButtonInMapIsPressed)
				{
					shiftVector.X = mouseDownPoint.X - e.X;
					shiftVector.Y = mouseDownPoint.Y - e.Y;
					mapPanel.Refresh();
				}
			}

		#endregion

		#region Model controls events

			private void modelStartButton_Click(object sender, EventArgs e)
			{
				if (GraphicalUIShell.Model.State == ModelState.Initialised)
				{                
					modelStartButton.Text = "Stop!";
					modelPauseButton.Text = "Pause";
					modelPauseButton.Enabled = true;
					GraphicalUIShell.Model.Start();
				}
				else if (GraphicalUIShell.Model.State == ModelState.Running || GraphicalUIShell.Model.State == ModelState.Paused || GraphicalUIShell.Model.State == ModelState.Error)
				{                
					modelStartButton.Text = "Initialize";
					modelPauseButton.Enabled = false;
					GraphicalUIShell.Model.Stop();
				}
				else if (GraphicalUIShell.Model.State == ModelState.Uninitialised)
				{                
					modelStartButton.Text = "Start";
					modelPauseButton.Enabled = false;
					GraphicalUIShell.Model.Initialise();
				}
			}

			private void modelPauseButton_Click(object sender, EventArgs e)
			{
				if (GraphicalUIShell.Model.State == ModelState.Running)
				{
					GraphicalUIShell.Model.Pause();
					modelPauseButton.Text = "Resume";
				}
				else if (GraphicalUIShell.Model.State == ModelState.Paused)
				{
					GraphicalUIShell.Model.Resume();
					modelPauseButton.Text = "Pause";
				}
			}     
			
			private void trackBarModelTick_Scroll(object sender, EventArgs e)
			{
				GraphicalUIShell.Model.ModelIterationTick = TimeSpan.FromMilliseconds(trackBarModelTick.Value * 40);
				labelModelTick.Text = "ModelTick = " + GraphicalUIShell.Model.ModelIterationTick.ToString();
			}

		#endregion

		#region Agents info controls

			private void clearInfobutton_Click(object sender, EventArgs e)
			{
				displayInfoAgents.Clear();
				mapInfoPanel.Refresh();
				mapPanel.Refresh();
			}

			/// <summary>
			/// Adding agents to the context menu to select wich agent info to show or select the agent if it is alone
			/// </summary>
			/// <param name="argCoords">UI coords to find agent</param>
			private void SelectAgentAt(Point argCoords)
			{
				currentAgentsInTheCell = GraphicalUIShell.Model.GetAgentsAt(GetModelCoord(argCoords)); //Agents that are in the cell with argCoords
				contextMenuSelectAgents.Items.Clear();
				
				//Adding new context menu elements to choose between several agents
				foreach (var ag in currentAgentsInTheCell)
				{
					var newMenuItemToAdd = new ToolStripMenuItem(ag.Name, null, new EventHandler(contextMenuAgentElement_Click));  

					if (displayInfoAgents.Contains(ag))
					{
						newMenuItemToAdd.Checked = true;
					}
					contextMenuSelectAgents.Items.Add(newMenuItemToAdd);
				}

				//Adding close element
				contextMenuSelectAgents.Items.Add("-");
				contextMenuSelectAgents.Items.Add(new ToolStripMenuItem("Ok", null, new EventHandler(contextMenuCloseElement_Click)));

				if(currentAgentsInTheCell.Count > 1)
				{
					contextMenuSelectAgents.Show(Cursor.Position);
				}

				if(currentAgentsInTheCell.Count == 1)
				{
					contextMenuSelectAgents.Close();

					if (displayInfoAgents.Contains(currentAgentsInTheCell[0]))
					{
						displayInfoAgents.Remove(currentAgentsInTheCell[0]);
					}
					else
					{
						displayInfoAgents.Add(currentAgentsInTheCell[0]);
					}
					mapInfoPanel.Refresh();
					mapPanel.Refresh();
				}
			}

			/// <summary>
			/// Event for click on context menu item to choose agents
			/// </summary>
			private void contextMenuAgentElement_Click(object sender, EventArgs e)
			{
				var convertedSender = sender as ToolStripMenuItem; //Sender menu item
				
				convertedSender.Checked = !convertedSender.Checked;
			}

			/// <summary>
			/// Event for click on context menu close button 
			/// </summary>
			private void contextMenuCloseElement_Click(object sender, EventArgs e)
			{
				for(int i = 0; i < contextMenuSelectAgents.Items.Count; i++)
				{			
					if(contextMenuSelectAgents.Items[i] as ToolStripMenuItem == null) //This is close element
					{
						break;
					}
							
					if (displayInfoAgents.Contains(currentAgentsInTheCell[i]))
					{
						if(!(contextMenuSelectAgents.Items[i] as ToolStripMenuItem).Checked)
						{
							displayInfoAgents.Remove(currentAgentsInTheCell[i]);
						}
					}
					else
					{
						if((contextMenuSelectAgents.Items[i] as ToolStripMenuItem).Checked)
						{
							displayInfoAgents.Add(currentAgentsInTheCell[i]);
						}
					}
				}

				contextMenuSelectAgents.Close();
				mapInfoPanel.Refresh();
				mapPanel.Refresh();
			}

		#endregion

    }
}
