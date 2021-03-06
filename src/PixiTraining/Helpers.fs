﻿namespace PixiTraining

open Fable.Core
open Fable.Import
open Fable.Import.PIXI
open Fable.PowerPack
open System

[<AutoOpen>]
module Helpers =

  type Behavior = Func<ESprite, float, bool>

  and ESprite(t: Texture, id: string, behaviors: Behavior list) =
    inherit Sprite(t)
    let mutable _behaviors = behaviors
    let mutable _disposed = false
    let mutable _prevTime = 0.

    member self.Id = id
    member self.IsDisposed = _disposed

    member self.AddBehavior(b:Behavior) =
      _behaviors <- b :: _behaviors

    member self.Update(dt: float) =
        let behaviors = _behaviors
        _behaviors <- []
        let mutable notCompletedBehaviors = []
        let dt =
          let tmp = _prevTime
          _prevTime <- dt
          if tmp = 0. then 0. else dt - tmp
        for b in behaviors do
          let complete = b.Invoke(self, dt)
          if not complete then
            notCompletedBehaviors <- b :: notCompletedBehaviors
        _behaviors <- _behaviors @ notCompletedBehaviors

    interface IDisposable with
      member self.Dispose() =
        if not _disposed then
          _disposed <- true
          self.parent.removeChild(self) |> ignore

  let makeSprite t =
    Sprite t

  let makeESprite (behaviors: Behavior list) id (t: Texture) =
    new ESprite(t, id, behaviors)

  let addToContainer (c: Container) (s: Sprite) =
    c.addChild s |> ignore
    s

  let setPosition x y (s: Container) =
    s.position <- Point(x, y)
    s

  let drawRect (g:Graphics) color width height =
    g.beginFill(float color) |> ignore
    g.drawRect(0., 0., width, height) |> ignore
    g.endFill() |> ignore
    g

  let setRotation angle (s: Sprite) =
    s.rotation <- angle
    s

  let centerPivot (s: Sprite) =
    let halfWidth = s.width / 2.
    let halfHeight = s.height / 2.
    s.pivot <- Point(halfWidth, halfHeight)
    s

  let createText txt : PIXI.Text =
    Text(txt)

  let setAnchor x y (s: Sprite) =
    s.anchor <- Point(x, y)
    s

  type Vector (?x, ?y) =
    let x = defaultArg x 0.
    let y = defaultArg y 0.

    member self.X
      with get () = x

    member self.Y
      with get () = y

    member self.Length() =
      Math.Sqrt(self.X * self.X + self.Y * self.Y)

    member self.Normalize() =
      let length = self.Length()
      new Vector(self.X / length, self.Y / length)

    static member (+) (a: Vector, b: Vector) =
      new Vector(a.X + b.X, a.Y + b.Y)

    static member (-) (a: Vector, b: Vector) =
      new Vector(a.X - b.X, a.Y - b.Y)

    static member (*) (s, a: Vector) =
      new Vector(s * a.X, s * a.Y)


    type Math with
      static member DegreesToRadian
        with get () = Math.PI / 180.

      static member RadianToDegrees
        with get () = 180. / Math.PI

