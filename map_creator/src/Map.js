import React from 'react';
import * as Konva from "konva";
import './Map.css';

export default class Map extends React.Component {

    constructor(props) {
        super(props);
        this.stage = null;
        this.layer = null;
        this.width = Math.floor(800);
        this.height = Math.floor(500);
        this.selectBox = this.selectBox.bind(this);
        this.dragstart = this.dragstart.bind(this);
        this.dragend = this.dragend.bind(this);
        this.imageOnLoad = this.imageOnLoad.bind(this)
    }
    componentDidMount() {
        var tween = null;
        var width = this.width;
        var height = this.height;

        this.stage = new Konva.Stage({
            container: this.containerRef,
            width: width,
            height: height,
            padding: 100
        });
        this.layer = new Konva.Layer();
        var dragLayer = new Konva.Layer();

        this.stage.add(this.layer, dragLayer);

        this.stage.on("dragstart", (e) => {
            this.dragstart(e, tween, dragLayer);
        });

        this.stage.on("dragend", (e) => {
            this.dragend(e);
        })
    }

    dragstart(e, tween, dragLayer) {
        const shape = e.target;
        // moving to another layer will improve dragging performance
        shape.moveTo(dragLayer);
        this.stage.draw();

        if (tween) {
            tween.pause();
        }

        shape.setAttrs({
            shadowOffset: {
                x: 10,
                y: 10
            },
            scale: {
                x: 1.2,
                y: 1.2
            }
        });
    }

    dragend(e) {
        const image = e.target;
        image.moveTo(this.layer);
        this.stage.draw();
        console.log("Image X: " + image.x() + ", Image Y: " + image.y());
        image.to({
            duration: 1,
            easing: Konva.Easings.ElasticEaseOut,
            scaleX: 1,
            scaleY: 1,
            shadowOffsetX: 0,
            shadowOffsetY: 0
        });
    }

    render() {
        return (
            <div>
                <img src="./box1.png" alt="box" className="box" onClick={() => this.selectBox("./box1.png")} />
                <img src="./box2.png" alt="box" className="box" onClick={() => this.selectBox("./box2.png")} />
                <div
                    className="container"
                    ref={ref => {
                        this.containerRef = ref;
                    }}
                />
            </div>
        );
    }

    selectBox(path) {
        this.addImage(path)
    }

    imageOnLoad(imageObj) {
        var scale = 1;
        var image = new Konva.Image({
            x: Math.abs(Math.random() * this.stage.getWidth() - 100),
            y: Math.abs(Math.random() * this.stage.getHeight() - 100),
            scale: {
                x: scale,
                y: scale
            },
            shadowOffset: {
                x: 0,
                y: 0
            },
            width: 100,
            height: 100,
            image: imageObj,
            draggable: true,
        });

        image.on('mouseover', function () {
            document.body.style.cursor = 'pointer';
        });
        image.on('mouseout', function () {
            document.body.style.cursor = 'default';
        });
        this.layer.add(image);
        this.stage.draw()
    }

    addImage(path) {

        var imageObj = new Image();
        imageObj.src = path
        imageObj.onload = () => {
            this.imageOnLoad(imageObj);
        };
    }
}