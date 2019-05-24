using System;
using System.Collections.Generic;
using System.Linq;
namespace PxBook
{
	public class Engine{
		internal class Variable
		{
			public string value, type;
			public Variable(string value,string type)
			{
				this.value = value;
				this.type = type.ToLower();
			}
			public override string ToString()
			{
				return value + "	("+type+")";
			}
		}
		Dictionary<string, Variable> variables;
		xPacket p;
		public Engine()
		{
			variables = new Dictionary<string, Variable>();
			p = null;
		}
		public void ParsingHandler(byte[] bytes, string[] script)
		{
			variables.Clear();
			p = new xPacket(bytes);
			string result;
			bool error = false;
      try
			{
				result = Parse(script, 0, script.Length-1);
			}
			catch(Exception ex)
			{
				error = true;
				result = ex.ToString();
			}
			Window.InvokeIfRequired(Window.Get.rtbxProcess, () => {
				Window.Get.rtbxProcess.Text = result;
			});
			// Add all variables to a viewer
			if (!error)
				ShowVariables();
		}
		public string Parse(string[] lines,int startLine,int endLine,string looping="")
		{
			string r = "";
			for (int i= startLine; i<=endLine; i++)
			{
				// Avoid comments
				if (lines[i].Trim().StartsWith("//"))
					continue;

				string[] lineValues = lines[i].Split(new string[] { " ", "	" }, StringSplitOptions.RemoveEmptyEntries);
				if (lineValues.Length == 2)
				{
					if (lineValues[0].ToLower() == "for")
					{
						// Parsing Conditional FOR
						string val = ParseValue(lineValues[1]);
						if (val == null)
						{
							val = lineValues[1];
							try
							{
								val = variables[lineValues[1]+looping].value;
							}
							catch { }
						}
						int n = 0;
						try
						{
							n = int.Parse(val);
						}
						catch { throw new Exception("For counter is not a numeric value"); }
						int endfor = getConditionalEnd("for", lines, i + 1, endLine);

						r += "[FOR][Count:" + n + "]" + Environment.NewLine;
						for (int loop = 0; loop < n; loop++)
						{
							r += Parse(lines, i + 1, endfor - 1, looping + "[" + loop + "]");
						}
						r += "[ENDFOR][From line " + (i + 1) + "]\n";
						i = endfor;
					}
					else
					{
						// Parsing only variables
						try
						{
							string val = ParseValue(lineValues[0]);
							if (val != null)
							{
								variables[lineValues[1] + looping] = new Variable(val, lineValues[0]);
								r += variables[lineValues[1] + looping].ToString() + Environment.NewLine;
							}
							else
							{
								r += "//" + lines[i] + Environment.NewLine;
							}
						}
						catch (Exception ex)
						{
							return r + ex.Message;
						}
					}
				}
				else if (lineValues.Length == 4)
				{
					// Parsing Conditional IF
					if (lineValues[0].ToLower() == "if") {
						string a = ParseValue(lineValues[1]);
						if (a == null)
						{
							a = lineValues[1];
							try
							{
								a = variables[lineValues[1]+looping].value;
							}
							catch { }
						}
						string b = ParseValue(lineValues[3]);
						if (b == null)
						{
							b = lineValues[3];
							try
							{
								b = variables[lineValues[3] + looping].value;
							}
							catch { }
						}
						string c = lineValues[2]; // condition
           
						// try to parse condition values
						int endif = getConditionalEnd("if", lines,i+1,endLine);

						r += "[IF "+ lineValues[1] + lineValues[2] + lineValues[3]+"]";
						if (ParseCondition(c, a, b))
						{
							r += "[True]" + Environment.NewLine;
							r += Parse(lines, i + 1, endif-1,looping);
						}
						else
						{
							r += "[False]" + Environment.NewLine;
						}
						r += "[ENDIF][From line " + (i+1) + "]\n";
						i = endif;
					}
					else if(lineValues[0].ToLower() == "while")
					{
						r += "[WHILE " + lineValues[1] + lineValues[2] + lineValues[3] + "]\n";
						int loop = 0;
						while (true) {
							string a = ParseValue(lineValues[1]);
							if (a == null)
							{
								a = lineValues[1];
								try
								{
									a = variables[lineValues[1]+looping].value;
								}
								catch { }
							}
							string b = ParseValue(lineValues[3]);
							if (b == null)
							{
								b = lineValues[3];
								try
								{
									b = variables[lineValues[3] + looping].value;
								}
								catch { }
							}
							string c = lineValues[2];

							int endwhile = getConditionalEnd("while", lines, i + 1, endLine);
							if (ParseCondition(c, a, b))
							{
								//r += "[True]" + Environment.NewLine;
								r += Parse(lines, i + 1, endwhile - 1,looping);
								r += "- - - - - - - "+ (loop + 1) + "\n";
								loop++;
								continue;
							}
							else
							{
								//r += "[False]" + Environment.NewLine;
							}
							r += "[ENDWHILE][From line " + (i + 1) + "]\n";
							i = endwhile;
							break;
						}
					}
					else
					{
						r += "//" + lines[i] + Environment.NewLine;
					}
				}
				else
				{
					r += "//" + lines[i] + Environment.NewLine;
				}
			}
			return r;
		}
		private string ParseValue(string type)
		{
			switch (type.ToLower())
			{
				case "byte":
				case "uint8":
					return p.ReadUInt8().ToString();
				case "ushort":
				case "uint16":
					return p.ReadUInt16().ToString();
				case "uint":
				case "uint32":
					return p.ReadUInt32().ToString();
				case "ulong":
				case "uint64":
					return p.ReadUInt64().ToString();
				case "single":
				case "float":
					return p.ReadSingle().ToString();
				case "ascii":
				case "ascii16":
					return p.ReadAscii();
				case "ascii8":
					return p.ReadAscii8();
				case "ascii32":
					return p.ReadAscii32();
				default:
					return null;
			}
		}
		private bool ParseCondition(string condition, string a, string b)
		{
			switch (condition)
			{
				case "==":
					if (a == b)
						return true;
					else
						return false;
				case "!=":
					if (a != b)
						return true;
					else
						return false;
				case ">":
					if (int.Parse(a) > int.Parse(b))
						return true;
					else
						return false;
				case "<":
					if (int.Parse(a) < int.Parse(b))
						return true;
					else
						return false;
				case ">=":
					if (int.Parse(a) >= int.Parse(b))
						return true;
					else
						return false;
				case "<=":
					if (int.Parse(a) <= int.Parse(b))
						return true;
					else
						return false;
			}
			throw new Exception("Syntax error: Invalid Script Conditional");
		}
		private int getConditionalEnd(string condition,string[] lines,int startLine, int endLine)
		{
			int levels = 0;
			for (int i=startLine; i<=endLine; i++)
			{
				string line = lines[i].ToLower().Trim();
				if (line.StartsWith(condition+" "))
				{
					levels++;
				}
				if (line.StartsWith("end"+ condition))
				{
					if (levels == 0)
					{
						return i;
					}
					levels--;
				}
			}
			throw new Exception("Syntax error: Ending "+condition.ToUpper()+" conditional not found. (Line "+ startLine + ")");
		}
		
		public static bool isParseable(string text)
		{
			switch (text.ToLower())
			{
				case "byte":
				case "uint8":
				case "ushort":
				case "uint16":
				case "uint":
				case "uint32":
				case "ulong":
				case "uint64":
				case "single":
				case "float":
				case "ascii":
				case "ascii16":
				case "ascii8":
				case "ascii32":
					return true;
			}
			return false;
		}
		public static bool isCondition(string key,string condition="")
		{
			switch (key.ToLower())
			{
				case "if":
				case "while":
					if (condition == "==" || condition == "!="
						|| condition == ">" || condition == ">="
						|| condition == "<" || condition == "<=" )
					{
						return true;
					}
					return false;
				case "for":
				case "endwhile":
				case "endif":
				case "endfor":
					return true;
			}
			return false;
		}
		public void ShowVariables()
		{
			Window w = Window.Get;
			Window.InvokeIfRequired(w.lstvVariables, () => {
				w.lstvVariables.Items.Clear();
			});
			Window.InvokeIfRequired(w.lstvVariables, () => {
				foreach (string key in variables.Keys)
					w.AddVariable(key, variables[key].type, variables[key].value);
			});
		}
	}
}
