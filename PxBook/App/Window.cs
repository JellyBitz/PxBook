using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace PxBook
{
	public partial class Window : Form
	{
		private Engine engine;
		private static Window w = null;
		public static Window Get
		{
			get
			{
				if (w == null)
					w = new Window();
				return w;
			}
		}
		public static void InvokeIfRequired(Control control, MethodInvoker action)
		{
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}
		private Window()
		{
			InitializeComponent();
			this.Font = new Font("Source Sans Pro", 18f);

			InitializeFonts(this);
			engine = new Engine();
			// Making all label transparent on image
			lblAbout_Logo.Parent = imgBackgroundAbout;
			lblAbout_Logo.BackColor = Color.Transparent;
			lblAbout_Logo01.Parent = imgBackgroundAbout;
			lblAbout_Logo01.BackColor = Color.Transparent;
			lblAbout_Logo02.Parent = imgBackgroundAbout;
			lblAbout_Logo02.BackColor = Color.Transparent;
			lblAbout_Text01.Parent = imgBackgroundAbout;
			lblAbout_Text01.BackColor = Color.Transparent;
			lblAbout_Text02.Parent = imgBackgroundAbout;
			lblAbout_Text02.BackColor = Color.Transparent;
		}
		private void InitializeFonts(Control c)
		{
			for (int i = 0; i < c.Controls.Count; i++)
			{
				// Using TAG to load custom fonts from WindowsForm
				c.Controls[i].Font = Fonts.Get.Load(c.Controls[i].Font,(string)c.Controls[i].Tag);
				InitializeFonts(c.Controls[i]);
			}
		}
		private void Window_Load(object sender, EventArgs e)
		{
			this.TabPageH_Option_Click(TabPageH_Control01_Option01,null);
			if (File.Exists("hexdump.txt")) {
				this.rtbxPacketHexDump.Text = File.ReadAllText("hexdump.txt");
			}
			ReloadLibrary();
    }
		// TabPage Horizontal
		private void TabPageH_Option_Click(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			if (c.Parent.Tag != null)
			{
				Control currentOption = (Control)c.Parent.Tag;
				if (currentOption.Name == c.Name)
					return;
				currentOption.BackColor = c.Parent.Parent.BackColor;
				c.Parent.Parent.Controls[currentOption.Name + "_Panel"].Visible = false;
			}
			c.Parent.Tag = c;
			c.BackColor = c.Parent.BackColor;
			c.Parent.Parent.Controls[c.Name + "_Panel"].Visible = true;;
		}
		// Focus
		private void Control_FocusEnter(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			string[] controlTags = new string[] { "cbx", "cmbx", "rtbx", "tbx","lstv" };
			foreach (string tag in controlTags)
			{
				if (c.Name.Contains(tag))
				{
					c.Parent.Controls[c.Name.Replace(tag, "lbl")].BackColor = Color.FromArgb(30, 150, 220);
					break;
				}
			}
		}
		private void Control_FocusLeave(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			string[] controlTags = new string[] { "cbx", "cmbx", "rtbx", "tbx","lstv" };
			foreach (string tag in controlTags)
			{
				if (c.Name.Contains(tag))
				{
					c.Parent.Controls[c.Name.Replace(tag, "lbl")].BackColor = c.Parent.BackColor;
					break;
				}
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			switch (c.Name)
			{
				case "btnParse":
					if (rtbxPacketHexDump.Text != "" && rtbxScript.Text != "")
					{
						byte[] bytesExtracted = null;
						try
						{
							string rawBytes = "";
							for (int i = 0; i < rtbxPacketHexDump.Lines.Length; i++)
							{
								MatchCollection matches;
								string pattern = "([0-9a-fA-F]{2})";
								// Check if it's hexdump type
								if (Regex.IsMatch(rtbxPacketHexDump.Lines[i],"[0-9]{10} "))
								{
									matches = Regex.Matches(rtbxPacketHexDump.Lines[i]," "+ pattern);
								}
								else
								{
									matches = Regex.Matches(rtbxPacketHexDump.Lines[i],pattern);
								}
								// Collect bytes
								foreach (Match match in matches)
								{
									rawBytes += match.ToString();
								}
							}
							rawBytes = rawBytes.Replace(" ", "");
							bytesExtracted = WindowsAPI.StringToByteArray(rawBytes);
            }
						catch (Exception ex){ /* Error extracting data */
							rtbxProcess.Text = ex.ToString();
							return;
						}
						string[] lines = rtbxScript.Lines;
						(new Thread((ThreadStart)delegate{
							engine.ParsingHandler(bytesExtracted, lines);
						})).Start();
					}
					break;
				case "btnSaveScript":
					if (tbxOpcode.Text.Trim() != "")
					{
						if (!Directory.Exists("Packets"))
							Directory.CreateDirectory("Packets");
						// Check if is an Opcode
						int hexNumber;
						string fileOut = tbxOpcode.Text.Trim().ToLower().Replace("0x", "");
						if (int.TryParse(fileOut, System.Globalization.NumberStyles.HexNumber, null, out hexNumber))
						{
							fileOut = "0x" + hexNumber.ToString("X4");
						}
						else
						{
							fileOut = tbxOpcode.Text.Trim();
            }
						fileOut = "Packets\\" + fileOut + ".pxbook";
						File.WriteAllText(fileOut,
							(this.tbxDesc.Text != "" ? "/// " + this.tbxDesc.Text + "\n" : "") +
							rtbxScript.Text);
					}
					break;
				case "btnLoadScript":
					if(tbxOpcode.Text.Trim() != "")
					{
						// Check if is an Opcode
						int hexNumber;
						string fileIn = tbxOpcode.Text.Trim().ToLower().Replace("0x","");
            if (int.TryParse(fileIn, System.Globalization.NumberStyles.HexNumber, null, out hexNumber))
						{
							fileIn = "0x" + hexNumber.ToString("X4");
						}
						fileIn = "Packets\\" + fileIn + ".pxbook";
						// Check if can load it
						if (File.Exists(fileIn))
						{
							rtbxScript.Text = File.ReadAllText(fileIn);
							if (rtbxScript.Text.StartsWith("/// "))
							{
								string[] split = rtbxScript.Text.Split(new char[] { '\n' }, 2);
								tbxDesc.Text = split[0].Replace("/// ", "");
								rtbxScript.Text = split[1];
							}
						}
					}
					break;
				case "btnSupportMe":
					Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=3L3UFLBN746AC");
					break;
			}
		}

		private void Control_TextChanged(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			switch (c.Name)
			{
				case "rtbxPacketHexDump":
					File.WriteAllText("HexDump.txt", rtbxPacketHexDump.Text);
					break;
			}
		}
		public void AddVariable(string name,string type,string data)
		{
			ListViewItem i = new ListViewItem(name);
			i.Name = name;
			i.SubItems.Add(type);
			i.SubItems.Add(data);
			this.lstvVariables.Items.Add(i);
		}
		private void ReloadLibrary()
		{
			lstvLibrary.Items.Clear();
      if (Directory.Exists("Packets"))
			{
        string[] files = Directory.GetFiles("Packets", "*.pxbook");
				for (int i=0;i<files.Length;i++)
				{
					string opcode = files[i].Substring(8, files[i].Length - 8 - 7); // "Packets\\[...].pxbook"
					ListViewItem item = new ListViewItem(opcode);
					item.Name = opcode;
					StreamReader fileReader = new StreamReader(files[i]);
					string description = fileReader.ReadLine();
          if (description.StartsWith("/// "))
					{
						item.SubItems.Add(description.Replace("/// ",""));
					}
					else
					{
						item.SubItems.Add("None");
					}
					lstvLibrary.Items.Add(item);
					fileReader.Close();
				}
			}
		}
		private void ListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListView l = (ListView)sender;
			switch (l.Name)
			{
				case "lstvLibrary":
					if (l.SelectedItems.Count == 1)
					{
						string filepath = "Packets\\"+l.SelectedItems[0].Text+".pxbook";
						if (File.Exists(filepath))
						{
							rtbxStructure.Clear();
							string[] lines = File.ReadAllLines(filepath);
							(new Thread((ThreadStart)delegate{
								rtbxStructure.Invoke((MethodInvoker)delegate{
									LoadPacketPrettyPrintting(lines);
								});
							})).Start();
						}
					}
					break;
			}
		}

		private void LoadPacketPrettyPrintting(string[] lines)
		{
			string level = "";
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim();
				string[] lineSplit = line.Split(new string[] { " ", "	" }, StringSplitOptions.RemoveEmptyEntries);
				// Pretty printting script
				if (line.StartsWith("//"))
				{
					// Comments
					if (!line.StartsWith("/// "))
					{
						RichTextBox_AppendText(rtbxStructure, level + line + "\n", Color.FromArgb(116, 112, 93));
					}
				}
				else if (lineSplit.Length == 1 && Engine.isCondition(lineSplit[0]))
				{
					// END conditions
					try
					{
						level = level.Remove(level.Length - 4);
					}
					catch { }
					RichTextBox_AppendText(rtbxStructure, level + lineSplit[0].ToLower() + "\n", Color.FromArgb(249, 36, 83));
				}
				else if (lineSplit.Length == 2 && Engine.isParseable(lineSplit[0]))
				{
					// VALUE check
					RichTextBox_AppendText(rtbxStructure, level + lineSplit[0].ToLower(), Color.FromArgb(79, 156, 190));
					rtbxStructure.AppendText(" " + lineSplit[1] + "\n");
				}
				else if (lineSplit.Length == 4 && Engine.isCondition(lineSplit[0], lineSplit[2]))
				{
					// IF / WHILE check
					RichTextBox_AppendText(rtbxStructure, level + lineSplit[0].ToLower(), Color.FromArgb(249, 36, 83));
					level += "    ";
					rtbxStructure.AppendText(" " + lineSplit[1]);
					RichTextBox_AppendText(rtbxStructure, " " + lineSplit[2] + " ", Color.FromArgb(249, 36, 83));
					rtbxStructure.AppendText(lineSplit[3] + "\n");
				}
				else if (lineSplit.Length == 2 && Engine.isCondition(lineSplit[0]))
				{
					// FOR check
					RichTextBox_AppendText(rtbxStructure, level + lineSplit[0].ToLower(), Color.FromArgb(249, 36, 83));
					level += "    ";
					rtbxStructure.AppendText(" " + lineSplit[1]);
					rtbxStructure.AppendText("\n");
				}
			}
		}

		private void RichTextBox_AppendText(RichTextBox rtbx,string text,Color c)
		{
			rtbx.SelectionStart = rtbxStructure.TextLength;
			rtbx.SelectionLength = 0;
			rtbx.SelectionColor = c;
			rtbx.AppendText(text);
			rtbx.SelectionColor = rtbxStructure.ForeColor;
		}
		private void Menu_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem t = (ToolStripMenuItem)sender;
			switch (t.Name)
			{
				case "MenuLibrary_Delete":
					if (lstvLibrary.SelectedItems.Count == 1)
					{
						string filepath = "Packets\\" + lstvLibrary.SelectedItems[0].Text + ".pxbook";
						if (File.Exists(filepath))
							File.Delete(filepath);
						lstvLibrary.SelectedItems[0].Remove();
					}
					break;
				case "MenuLibrary_Reload":
					ReloadLibrary();
					break;
				case "MenuLibrary_Open":
					if (!Directory.Exists("Packets"))
						Directory.CreateDirectory("Packets");
					Process.Start(Environment.CurrentDirectory+"\\Packets");
					break;
			}
		}
	}
}
