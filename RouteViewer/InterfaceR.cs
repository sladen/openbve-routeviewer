﻿// ╔═════════════════════════════════════════════════════════════╗
// ║ Interface.cs for the Route Viewer                           ║
// ╠═════════════════════════════════════════════════════════════╣
// ║ This file cannot be used in the openBVE main program.       ║
// ║ The file from the openBVE main program cannot be used here. ║
// ╚═════════════════════════════════════════════════════════════╝

using System;
using System.Globalization;
using Tao.Sdl;

namespace OpenBve {

	// --- TimeTable.cs ---
	internal static class Timetable {
		internal static string TimetableDescription;
		internal static int[] CustomTextureIndices = null;
		internal static bool CustomVisible = false;
	}

	// --- PluginManager.cs ---
	internal static class PluginManager {
		internal static int[] PluginPanel = new int[] { };
	}

	// --- Interface.cs ---
	internal static class Interface {

		// special folders
		internal static string GetDataFolder(params string[] Subfolders) {
			if (Program.UseFilesystemHierarchyStandard) {
				string Folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				Folder = GetCombinedFolderName(Folder, "games");
				Folder = GetCombinedFolderName(Folder, "OpenBve");
				Folder = GetCombinedFolderName(Folder, "Data");
				for (int i = 0; i < Subfolders.Length; i++) {
					Folder = GetCombinedFolderName(Folder, Subfolders[i]);
				}
				return Folder;
			} else {
				string Folder = GetCombinedFolderName(System.Windows.Forms.Application.StartupPath, "Data");
				for (int i = 0; i < Subfolders.Length; i++) {
					Folder = GetCombinedFolderName(Folder, Subfolders[i]);
				}
				return Folder;
			}
		}

		// ================================

		// options
		internal enum SoundRange {
			Low = 0,
			Medium = 1,
			High = 2
		}
		internal struct Options {
			internal TextureManager.InterpolationMode Interpolation;
			internal int AnisotropicFilteringLevel;
			internal int AnisotropicFilteringMaximum;
			internal Renderer.TransparencyMode TransparencyMode;
			internal SoundRange SoundRange;
			internal int SoundNumber;
			internal bool UseSound;
			internal int ObjectOptimizationBasicThreshold;
			internal int ObjectOptimizationFullThreshold;
		}
		internal static Options CurrentOptions;

		// messages
		internal enum MessageType {
			Information = 1,
			Warning = 2,
			Error = 3,
			Critical = 4
		}
		internal struct Message {
			internal MessageType Type;
			internal bool FileNotFound;
			internal string Text;
		}
		internal static Message[] Messages = new Message[] { };
		internal static int MessageCount = 0;
		internal static void AddMessage(MessageType Type, bool FileNotFound, string Text) {
			if (MessageCount == 0) {
				Messages = new Message[16];
			} else if (MessageCount >= Messages.Length) {
				Array.Resize<Message>(ref Messages, Messages.Length << 1);
			}
			Messages[MessageCount].Type = Type;
			Messages[MessageCount].FileNotFound = FileNotFound;
			Messages[MessageCount].Text = Text;
			MessageCount++;
		}
		internal static void ClearMessages() {
			Messages = new Message[] { };
			MessageCount = 0;
		}

		// ================================

		// try parse vb6
		internal static bool TryParseDoubleVb6(string Expression, out double Value) {
			Expression = TrimInside(Expression);
			CultureInfo Culture = CultureInfo.InvariantCulture;
			for (int n = Expression.Length; n > 0; n--) {
				double a;
				if (double.TryParse(Expression.Substring(0, n), NumberStyles.Float, Culture, out a)) {
					Value = a;
					return true;
				}
			}
			Value = 0.0;
			return false;
		}
		internal static bool TryParseFloatVb6(string Expression, out float Value) {
			Expression = TrimInside(Expression);
			CultureInfo Culture = CultureInfo.InvariantCulture;
			for (int n = Expression.Length; n > 0; n--) {
				float a;
				if (float.TryParse(Expression.Substring(0, n), NumberStyles.Float, Culture, out a)) {
					Value = a;
					return true;
				}
			}
			Value = 0.0f;
			return false;
		}
		internal static bool TryParseIntVb6(string Expression, out int Value) {
			Expression = TrimInside(Expression);
			CultureInfo Culture = CultureInfo.InvariantCulture;
			for (int n = Expression.Length; n > 0; n--) {
				double a;
				if (double.TryParse(Expression.Substring(0, n), NumberStyles.Float, Culture, out a)) {
					if (a >= -2147483648.0 & a <= 2147483647.0) {
						Value = (int)Math.Round(a);
						return true;
					} else break;
				}
			}
			Value = 0;
			return false;
		}
		internal static bool TryParseByteVb6(string Expression, out byte Value) {
			Expression = TrimInside(Expression);
			CultureInfo Culture = CultureInfo.InvariantCulture;
			for (int n = Expression.Length; n > 0; n--) {
				double a;
				if (double.TryParse(Expression.Substring(0, n), NumberStyles.Float, Culture, out a)) {
					if (a >= 0.0 & a <= 255.0) {
						Value = (byte)Math.Round(a);
						return true;
					} else break;
				}
			}
			Value = 0;
			return false;
		}

		// try parse time
		internal static bool TryParseTime(string Expression, out double Value) {
			Expression = TrimInside(Expression);
			if (Expression.Length != 0) {
				CultureInfo Culture = CultureInfo.InvariantCulture;
				int i = Expression.IndexOf('.');
				if (i >= 1) {
					int h; if (int.TryParse(Expression.Substring(0, i), NumberStyles.Integer, Culture, out h)) {
						int n = Expression.Length - i - 1;
						if (n == 1 | n == 2) {
							uint m; if (uint.TryParse(Expression.Substring(i + 1, n), NumberStyles.None, Culture, out m)) {
								Value = 3600.0 * (double)h + 60.0 * (double)m;
								return true;
							}
						} else if (n == 3 | n == 4) {
							uint m; if (uint.TryParse(Expression.Substring(i + 1, 2), NumberStyles.None, Culture, out m)) {
								uint s; if (uint.TryParse(Expression.Substring(i + 3, n - 2), NumberStyles.None, Culture, out s)) {
									Value = 3600.0 * (double)h + 60.0 * (double)m + (double)s;
									return true;
								}
							}
						}
					}
				} else if (i == -1) {
					int h; if (int.TryParse(Expression, NumberStyles.Integer, Culture, out h)) {
						Value = 3600.0 * (double)h;
						return true;
					}
				}
			}
			Value = 0.0;
			return false;
		}

		// try parse hex color
		internal static bool TryParseHexColor(string Expression, out World.ColorRGB Color) {
			if (Expression.StartsWith("#")) {
				string a = Expression.Substring(1).TrimStart();
				int x; if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out x)) {
					int r = (x >> 16) & 0xFF;
					int g = (x >> 8) & 0xFF;
					int b = x & 0xFF;
					if (r >= 0 & r <= 255 & g >= 0 & g <= 255 & b >= 0 & b <= 255) {
						Color = new World.ColorRGB((byte)r, (byte)g, (byte)b);
						return true;
					} else {
						Color = new World.ColorRGB(0, 0, 255);
						return false;
					}
				} else {
					Color = new World.ColorRGB(0, 0, 255);
					return false;
				}
			} else {
				Color = new World.ColorRGB(0, 0, 255);
				return false;
			}
		}
		internal static bool TryParseHexColor(string Expression, out World.ColorRGBA Color) {
			if (Expression.StartsWith("#")) {
				string a = Expression.Substring(1).TrimStart();
				int x; if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out x)) {
					int r = (x >> 16) & 0xFF;
					int g = (x >> 8) & 0xFF;
					int b = x & 0xFF;
					if (r >= 0 & r <= 255 & g >= 0 & g <= 255 & b >= 0 & b <= 255) {
						Color = new World.ColorRGBA((byte)r, (byte)g, (byte)b, 255);
						return true;
					} else {
						Color = new World.ColorRGBA(0, 0, 255, 255);
						return false;
					}
				} else {
					Color = new World.ColorRGBA(0, 0, 255, 255);
					return false;
				}
			} else {
				Color = new World.ColorRGBA(0, 0, 255, 255);
				return false;
			}
		}

		// try parse with unit factors
		internal static bool TryParseDouble(string Expression, double[] UnitFactors, out double Value) {
			double a;
			if (double.TryParse(Expression, NumberStyles.Any, CultureInfo.InvariantCulture, out a)) {
				Value = a;
				return true;
			} else {
				int j = 0, n = 0; Value = 0;
				for (int i = 0; i < Expression.Length; i++) {
					if (Expression[i] == ':') {
						string t = Expression.Substring(j, i - j);
						if (double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out a)) {
							if (n < UnitFactors.Length) {
								Value += a * UnitFactors[n];
							} else {
								return n > 0;
							}
						} else {
							return n > 0;
						} j = i + 1; n++;
					}
				}
				{
					string t = Expression.Substring(j);
					if (double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out a)) {
						if (n < UnitFactors.Length) {
							Value += a * UnitFactors[n];
							return true;
						} else {
							return n > 0;
						}
					} else {
						return n > 0;
					}
				}
			}
		}
		internal static bool TryParseDoubleVb6(string Expression, double[] UnitFactors, out double Value) {
			double a;
			if (double.TryParse(Expression, NumberStyles.Any, CultureInfo.InvariantCulture, out a)) {
				Value = a;
				return true;
			} else {
				int j = 0, n = 0; Value = 0;
				for (int i = 0; i < Expression.Length; i++) {
					if (Expression[i] == ':') {
						string t = Expression.Substring(j, i - j);
						if (TryParseDoubleVb6(t, out a)) {
							if (n < UnitFactors.Length) {
								Value += a * UnitFactors[n];
							} else {
								return n > 0;
							}
						} else {
							return n > 0;
						} j = i + 1; n++;
					}
				}
				{
					string t = Expression.Substring(j);
					if (TryParseDoubleVb6(t, out a)) {
						if (n < UnitFactors.Length) {
							Value += a * UnitFactors[n];
							return true;
						} else {
							return n > 0;
						}
					} else {
						return n > 0;
					}
				}
			}
		}

		// trim inside
		private static string TrimInside(string Expression) {
			System.Text.StringBuilder Builder = new System.Text.StringBuilder(Expression.Length);
			for (int i = 0; i < Expression.Length; i++) {
				char c = Expression[i];
				if (!char.IsWhiteSpace(c)) {
					Builder.Append(c);
				}
			} return Builder.ToString();
		}

		// is japanese
		internal static bool IsJapanese(string Name) {
			for (int i = 0; i < Name.Length; i++) {
				int a = char.ConvertToUtf32(Name, i);
				if (a < 0x10000) {
					bool q = false;
					while (true) {
						if (a >= 0x2E80 & a <= 0x2EFF) break;
						if (a >= 0x3000 & a <= 0x30FF) break;
						if (a >= 0x31C0 & a <= 0x4DBF) break;
						if (a >= 0x4E00 & a <= 0x9FFF) break;
						if (a >= 0xF900 & a <= 0xFAFF) break;
						if (a >= 0xFE30 & a <= 0xFE4F) break;
						if (a >= 0xFF00 & a <= 0xFFEF) break;
						q = true; break;
					} if (q) return false;
				} else {
					return false;
				}
			} return true;
		}

		// unescape
		internal static string Unescape(string Text) {
			System.Text.StringBuilder Builder = new System.Text.StringBuilder(Text.Length);
			int Start = 0;
			for (int i = 0; i < Text.Length; i++) {
				if (Text[i] == '\\') {
					Builder.Append(Text, Start, i - Start);
					if (i + 1 <= Text.Length) {
						switch (Text[i + 1]) {
								case 'a': Builder.Append('\a'); break;
								case 'b': Builder.Append('\b'); break;
								case 't': Builder.Append('\t'); break;
								case 'n': Builder.Append('\n'); break;
								case 'v': Builder.Append('\v'); break;
								case 'f': Builder.Append('\f'); break;
								case 'r': Builder.Append('\r'); break;
								case 'e': Builder.Append('\x1B'); break;
							case 'c':
								if (i + 2 < Text.Length) {
									int CodePoint = char.ConvertToUtf32(Text, i + 2);
									if (CodePoint >= 0x40 & CodePoint <= 0x5F) {
										Builder.Append(char.ConvertFromUtf32(CodePoint - 64));
									} else if (CodePoint == 0x3F) {
										Builder.Append('\x7F');
									} else {
										Interface.AddMessage(MessageType.Error, false, "Unrecognized control character found in " + Text.Substring(i, 3));
										return Text;
									} i++;
								} else {
									Interface.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode control character escape sequence");
									return Text;
								} break;
								case '"': Builder.Append('"'); break;
								case '\\': Builder.Append('\\'); break;
							case 'x':
								if (i + 3 < Text.Length) {
									Builder.Append(char.ConvertFromUtf32(Convert.ToInt32(Text.Substring(i + 2, 2), 16)));
									i += 2;
								} else {
									Interface.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode hexadecimal escape sequence.");
									return Text;
								} break;
							case 'u':
								if (i + 5 < Text.Length) {
									Builder.Append(char.ConvertFromUtf32(Convert.ToInt32(Text.Substring(i + 2, 4), 16)));
									i += 4;
								} else {
									Interface.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode hexadecimal escape sequence.");
									return Text;
								} break;
							default:
								Interface.AddMessage(MessageType.Error, false, "Unrecognized escape sequence found in " + Text + ".");
								return Text;
						}
						i++; Start = i + 1;
					} else {
						Interface.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode escape sequence.");
						return Text;
					}
				}
			}
			Builder.Append(Text, Start, Text.Length - Start);
			return Builder.ToString();
		}

		// ================================

		// round to power of two
		internal static int RoundToPowerOfTwo(int Value) {
			Value -= 1;
			for (int i = 1; i < sizeof(int) * 8; i *= 2) {
				Value = Value | Value >> i;
			} return Value + 1;
		}

		// convert newlines to crlf
		internal static string ConvertNewlinesToCrLf(string Text) {
			System.Text.StringBuilder Builder = new System.Text.StringBuilder();
			for (int i = 0; i < Text.Length; i++) {
				int a = char.ConvertToUtf32(Text, i);
				if (a == 0xD & i < Text.Length - 1) {
					int b = char.ConvertToUtf32(Text, i + 1);
					if (b == 0xA) {
						Builder.Append("\r\n");
						i++;
					} else {
						Builder.Append("\r\n");
					}
				} else if (a == 0xA | a == 0xC | a == 0xD | a == 0x85 | a == 0x2028 | a == 0x2029) {
					Builder.Append("\r\n");
				} else if (a < 0x10000) {
					Builder.Append(Text[i]);
				} else {
					Builder.Append(Text.Substring(i, 2));
					i++;
				}
			} return Builder.ToString();
		}

		// ================================

		// get corrected path separation
		internal static string GetCorrectedPathSeparation(string Expression) {
			if (Program.CurrentPlatform == Program.Platform.Windows) {
				if (Expression.Length != 0 && Expression[0] == '\\') {
					return Expression.Substring(1);
				} else {
					return Expression;
				}
			} else {
				if (Expression.Length != 0 && Expression[0] == '\\') {
					return Expression.Substring(1).Replace("\\", new string(new char[] { System.IO.Path.DirectorySeparatorChar }));
				} else {
					return Expression.Replace("\\", new string(new char[] { System.IO.Path.DirectorySeparatorChar }));
				}
			}
		}

		// get corected folder name
		internal static string GetCorrectedFolderName(string Folder) {
			if (Folder.Length == 0) {
				return "";
			} else if (Program.CurrentPlatform == Program.Platform.Linux) {
				// find folder case-insensitively
				if (System.IO.Directory.Exists(Folder)) {
					return Folder;
				} else {
					string Parent = GetCorrectedFolderName(System.IO.Path.GetDirectoryName(Folder));
					Folder = System.IO.Path.Combine(Parent, System.IO.Path.GetFileName(Folder));
					if (Folder != null && System.IO.Directory.Exists(Parent)) {
						if (System.IO.Directory.Exists(Folder)) {
							return Folder;
						} else {
							string[] Folders = System.IO.Directory.GetDirectories(Parent);
							for (int i = 0; i < Folders.Length; i++) {
								if (string.Compare(Folder, Folders[i], StringComparison.OrdinalIgnoreCase) == 0) {
									return Folders[i];
								}
							}
						}
					}
					return Folder;
				}
			} else {
				return Folder;
			}
		}
		
		// get corrected file name
		internal static string GetCorrectedFileName(string File) {
			if (File.Length == 0) {
				return "";
			} else if (Program.CurrentPlatform == Program.Platform.Linux) {
				// find file case-insensitively
				if (System.IO.File.Exists(File)) {
					return File;
				} else {
					string Folder = GetCorrectedFolderName(System.IO.Path.GetDirectoryName(File));
					File = System.IO.Path.Combine(Folder, System.IO.Path.GetFileName(File));
					if (System.IO.Directory.Exists(Folder)) {
						if (System.IO.File.Exists(File)) {
							return File;
						} else {
							string[] Files = System.IO.Directory.GetFiles(Folder);
							for (int i = 0; i < Files.Length; i++) {
								if (string.Compare(File, Files[i], StringComparison.OrdinalIgnoreCase) == 0) {
									return Files[i];
								}
							}
						}
					}
					return File;
				}
			} else {
				return File;
			}
		}

		// get combined file name
		internal static string GetCombinedFileName(string SafeFolderPart, string UnsafeFilePart) {
			return GetCorrectedFileName(System.IO.Path.Combine(SafeFolderPart, GetCorrectedPathSeparation(UnsafeFilePart)));
		}
		
		// get combined folder name
		internal static string GetCombinedFolderName(string SafeFolderPart, string UnsafeFolderPart) {
			return GetCorrectedFolderName(System.IO.Path.Combine(SafeFolderPart, GetCorrectedPathSeparation(UnsafeFolderPart)));
		}
		
		// contains invalid path characters
		internal static bool ContainsInvalidPathChars(string Expression) {
			char[] a = System.IO.Path.GetInvalidFileNameChars();
			char[] b = System.IO.Path.GetInvalidPathChars();
			for (int i = 0; i < Expression.Length; i++) {
				for (int j = 0; j < a.Length; j++) {
					if (Expression[i] == a[j]) {
						for (int k = 0; k < b.Length; k++) {
							if (Expression[i] == b[k]) {
								return true;
							}
						}
					}
				}
			}
			return false;
		}

	}
}