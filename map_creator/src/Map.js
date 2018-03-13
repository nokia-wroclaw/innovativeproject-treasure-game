import React from 'react';
import * as Konva from "konva";
import './Map.css';

export default class Map extends React.Component {

    constructor(props) {
        super(props);
        this.stage = null;
        this.layer = null;
    }
    componentDidMount() {
        //const tween = null;

        this.stage = new Konva.Stage({
            container: this.containerRef,
            width: 1024,
            height: 600
        });


        this.layer = new Konva.Layer();
        //const dragLayer = new Konva.Layer();

        this.stage.add(this.layer);

        /*stage.on("dragstart", function (evt) {
            const shape = evt.target;
            // moving to another layer will improve dragging performance
            shape.moveTo(dragLayer);
            stage.draw();

            if (tween) {
                tween.pause();
            }
            shape.setAttrs({
                shadowOffset: {
                    x: 15,
                    y: 15
                },
                scale: {
                    x: shape.getAttr("startScale") * 1.2,
                    y: shape.getAttr("startScale") * 1.2
                }
            });
        });

        stage.on("dragend", function (evt) {
            const shape = evt.target;
            shape.moveTo(layer);
            stage.draw();
            shape.to({
                duration: 0.5,
                easing: Konva.Easings.ElasticEaseOut,
                scaleX: shape.getAttr("startScale"),
                scaleY: shape.getAttr("startScale"),
                shadowOffsetX: 5,
                shadowOffsetY: 5
            });
        });*/
    }

    render() {
        return (
            <div
                className="container"
                ref={ref => {
                    console.log(ref);
                    this.containerRef = ref;
                }}
            />
        );
    }

    addImage() {
        var imageObj = new Image();
        imageObj.src = this.props.selectedElement;
        imageObj.misc = { stage: this.stage, layer: this.layer };
        console.log(this.stage)
        imageObj.onload = function () {
            var image = new Konva.Image({
                x: Math.random() * this.misc.stage.getWidth(),
                y: Math.random() * this.misc.stage.getHeight(),
                width: 100,
                height: 100,
                image: imageObj,
                draggable: true
            });
            this.misc.layer.add(image);
            this.misc.layer.draw();
        };
    }
}