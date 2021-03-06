﻿module Suave.Json

open System.IO

open System.Runtime.Serialization.Json
open System.Text

/// Convert the object to a JSON representation inside a byte array (can be made string of)
let to_json<'a> (o: 'a) =
  let dcs = DataContractJsonSerializer(o.GetType())
  let ms = new MemoryStream()
  dcs.WriteObject(ms, o)
  ms.ToArray()

/// Transform the byte array representing a JSON object to a .Net object
let from_json<'a> (bytes:byte []) =
  let dcs = DataContractJsonSerializer(typeof<'a>)
  let ms = new MemoryStream()
  ms.Write(bytes, 0, bytes.Length)
  ms.Seek(0L, SeekOrigin.Begin) |> ignore
  dcs.ReadObject(ms) :?> 'a

/// Expose function f through a json call; lets you write like
///
/// let app =
///   url "/path"  >>= request (map_json some_function);
///
let map_json f ( r : Types.HttpRequest) =
  f  (from_json(r.raw_form)) |> to_json |> Http.ok
