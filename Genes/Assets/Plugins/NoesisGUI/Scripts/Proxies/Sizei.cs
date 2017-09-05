/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.4
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;
using System.Runtime.InteropServices;

namespace Noesis
{

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct Sizei {

  [MarshalAs(UnmanagedType.U4)]
  private uint _w;

  [MarshalAs(UnmanagedType.U4)]
  private uint _h;

  public uint Width {
    get { return this._w; }
    set { this._w = value; }
  }

  public uint Height {
    get { return this._h; }
    set { this._h = value; }
  }

  public static Sizei Zero {
    get { return new Sizei(0, 0); }
  }

  public static Sizei Infinite {
    get { return new Sizei(System.UInt32.MaxValue, System.UInt32.MaxValue); }
  }

  public Sizei(uint w, uint h) {
    this._w = w;
    this._h = h;
  }

  public Sizei(Size size) : this((uint)size.Width, (uint)size.Height) {
  }

  public Sizei(Pointi point) : this((uint)point.X, (uint)point.Y) {
  }

  public static Sizei operator+(Sizei v0, Sizei v1) {
    return new Sizei(v0.Width + v1.Width, v0.Height + v1.Height);
  }

  public static Sizei operator-(Sizei v0, Sizei v1) {
    return new Sizei(v0.Width - v1.Width, v0.Height - v1.Height);
  }

  public static Sizei operator*(Sizei v, uint f) {
    return new Sizei(v.Width * f, v.Height * f);
  }

  public static Sizei operator*(uint f, Sizei v) {
    return v * f;
  }

  public static Sizei operator/(Sizei v, uint f) {
    if (f == 0) { throw new System.DivideByZeroException(); }
    return new Sizei(v.Width / f, v.Height / f);
  }

  public static bool operator==(Sizei v0, Sizei v1) {
    return v0.Width == v1.Width && v0.Height == v1.Height;
  }

  public static bool operator!=(Sizei v0, Sizei v1) {
    return !(v0 == v1);
  }

  public override bool Equals(System.Object obj) {
    return obj is Sizei && this == (Sizei)obj;
  }

  public bool Equals(Sizei v) {
    return this == v;
  }

  public override int GetHashCode() {
    return (int)Width ^ (int)Height;
  }

  public override string ToString() {
    return System.String.Format("{0},{1}", Width, Height);
  }

  public void Expand(Sizei size) {
    Width = System.Math.Max(Width, size.Width);
    Height = System.Math.Max(Height, size.Height);
  }

  public void Scale(uint scaleX, uint scaleY) {
    Width *= scaleX;
    Height *= scaleY;
  }

  public static Sizei Parse(string str) {
    Sizei size;
    if (Sizei.TryParse(str, out size)) {
      return size;
    }
    throw new ArgumentException("Cannot create Sizei from '" + str + "'");
  }

  public static bool TryParse(string str, out Sizei result) {
    bool ret = NoesisGUI_PINVOKE.Sizei_TryParse(str != null ? str : string.Empty, out result);
    #if UNITY_EDITOR || NOESIS_API
    if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
    #endif
    return ret;
  }

}

}
