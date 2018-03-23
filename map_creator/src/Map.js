import React from 'react';
import * as Konva from "konva";
import * as fs from "browser-filesaver";
import './Map.css';

export default class Map extends React.Component {

    constructor(props) {
        super(props);
        this.stage = null;
        this.layer = null;
        this.shadowRectangle = null;
        this.blockSize = 30;
        this.imageIndex = 0;
        this.objects = [];
        this.mapObject = { "box1": [], "box2": [], "piramid1": [] };
        this.width = Math.floor(800 / this.blockSize) * this.blockSize;
        this.height = Math.floor(500 / this.blockSize) * this.blockSize;
        this.bindMethods = this.bindMethods.bind(this);
        this.bindMethods();
        this.state = { width: this.width, height: this.height, blockSize: this.blockSize };
    }
    componentDidMount() {
        const tween = null;
        const width = this.width;
        const height = this.height;
        this.readAssets();

        this.shadowRectangle = new Konva.Rect({
            x: 0,
            y: 0,
            width: this.blockSize,
            height: this.blockSize,
            scale: {
                x: 1,
                y: 1
            },
            fill: '#07fc1b',
            opacity: 0.6,
            stroke: '#03bc12',
            strokeWidth: 3
        });

        this.stage = new Konva.Stage({
            container: this.containerRef,
            width: width,
            height: height,
            padding: 100
        });
        this.layer = new Konva.Layer();
        this.shadowRectangle.hide();
        this.layer.add(this.shadowRectangle);
        const dragLayer = new Konva.Layer();

        this.drawGrid();

        this.stage.add(this.layer, dragLayer);

        this.stage.on("dragstart", (e) => {
            this.dragstart(e, tween, dragLayer);
        });

        this.stage.on("dragend", (e) => {
            this.dragend(e);
        })
    }

    // Misc

    bindMethods() {
        this.selectBox = this.selectBox.bind(this);
        this.dragstart = this.dragstart.bind(this);
        this.dragend = this.dragend.bind(this);
        this.imageOnLoad = this.imageOnLoad.bind(this);
        this.drawGrid = this.drawGrid.bind(this);
        this.redraw = this.redraw.bind(this);
        this.generate = this.generate.bind(this);
        this.generateRandom = this.generateRandom.bind(this);
        this.checkPos = this.checkPos.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.strMapToObj = this.strMapToObj.bind(this);
        this.readAssets = this.readAssets.bind(this);
    }

    // Stage stuff

    drawGrid() {
        for (let i = 0; i < this.width / this.blockSize; i++) {
            this.layer.add(new Konva.Line({
                points: [Math.round(i * this.blockSize) + 0.5, 0, Math.round(i * this.blockSize) + 0.5, this.height],
                stroke: '#ddd',
                strokeWidth: 1,
            }));
        }

        for (let j = 0; j < this.height / this.blockSize; j++) {
            this.layer.add(new Konva.Line({
                points: [0, Math.round(j * this.blockSize), this.width, Math.round(j * this.blockSize)],
                stroke: '#ddd',
                strokeWidth: 1,
            }));
        }
    }

    redraw() {
        this.layer.removeChildren();
        this.drawGrid();
        this.layer.add(this.shadowRectangle);
        const pos = { x: -1, y: -1 };
        this.objects.forEach((entry) => {
            this.checkPos(entry, pos);
            entry.setAttrs({ x: pos.x, y: pos.y })
            entry.setHeight(this.blockSize * 4);
            entry.setWidth(this.blockSize * 4);
            this.layer.add(entry);
        });
        this.stage.batchDraw();
    }

    // Click events

    generate() {
        // this.readAssets();
        let i = 0;
        this.objects.forEach((entry) => {
            this.mapObject[entry.type].push({ "x": entry.x(), "y": entry.y() });
            var x = this.mapToJson(this.mapObject);
            console.log(x);
            console.log("Object " + entry.type + " X: " + entry.x() + ", Y: " + entry.y());
            i++;
        });
        // var x = this.mapToJson(this.mapObject);
        // var blob =
        //     new Blob(x, {type: 'text/plain;charset=utf-8'});
        // fs.saveAs(blob, 'XD.json');
        this.download();
    }

    generateRandom() {
        this.objects = [];
        const items = [['./assets/box1.png', 'box1'], ['./assets/box2.png', 'box2'], ['./assets/piramid1.png', 'piramid1']];
        for (let i = 0; i < 7; i++) {
            const randomElement = items[Math.floor(Math.random() * items.length)];
            this.addImage(randomElement[0], randomElement[1]);
        }
        this.redraw();
    }

    dragstart(e, tween, dragLayer) {
        const image = e.target;
        this.shadowRectangle.setAttrs({
            width: image.width(),
            height: image.height()
        })

        this.shadowRectangle.show();
        this.shadowRectangle.moveToTop();

        image.moveToTop();
        // moving to another layer will improve dragging performance
        image.moveTo(dragLayer);
        this.stage.draw();

        if (tween) {
            tween.pause();
        }

        image.setAttrs({
            shadowOffset: {
                x: 10,
                y: 10
            },
            scale: {
                x: 0.7,
                y: 0.7
            }
        });
    }

    dragend(e) {
        this.shadowRectangle.hide();
        const image = e.target;
        image.moveTo(this.layer);
        this.stage.draw();
        image.to({
            duration: 1,
            easing: Konva.Easings.ElasticEaseOut,
            scaleX: 1,
            scaleY: 1,
            shadowOffsetX: 0,
            shadowOffsetY: 0
        });
        image.position({
            x: this.shadowRectangle.x(),
            y: this.shadowRectangle.y()
        });
        console.log("Image X: " + image.x() + ", Image Y: " + image.y());
    }

    selectBox(path, type) {
        this.addImage(path, type)
    }

    handleChange(event) {

        const target = event.target;
        const value = event.target.value;
        const name = target.name;

        this.setState({
            [name]: value
        });
    }

    handleSubmit() {
        this.width = this.state.width;
        this.height = this.state.height;
        this.blockSize = this.state.blockSize;
        this.stage.setHeight(this.state.height);
        this.stage.setWidth(this.state.width);
        this.redraw();
    }

    // Image stuff 

    imageOnLoad(imageObj, type) {
        const scale = 1;
        const image = new Konva.Image({
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
            width: this.blockSize * 4,
            height: this.blockSize * 4,
            image: imageObj,
            draggable: true,
        });
        const pos = { x: -1, y: -1 };
        this.checkPos(image, pos)

        image.position({ x: pos.x, y: pos.y });

        image.type = type;
        image.src = imageObj.src;
        image.imageIndex = this.imageIndex;
        this.imageIndex++;

        image.on('mouseover', function () {
            document.body.style.cursor = 'pointer';
        });
        image.on('mouseout', function () {
            document.body.style.cursor = 'default';
        });

        image.on("dblclick", () => {
            delete this.objects[image.imageIndex];
            this.redraw();
        });

        image.on('dragmove', () => {
            const pos = { x: -1, y: -1 };
            this.checkPos(image, pos);

            this.shadowRectangle.position({
                x: pos.x,
                y: pos.y
            });
            this.stage.batchDraw();
        });
        this.objects.push(image);
        this.layer.add(image);
        this.stage.draw()
    }

    addImage(path, type) {

        const imageObj = new Image();
        imageObj.src = path
        imageObj.misc = { stage: this.stage, layer: this.layer };
        imageObj.onload = () => {
            this.imageOnLoad(imageObj, type);
        };
    }

    reloadImage(image) {
        this.layer.add(image);
        this.stage.batchDraw();
    }

    checkPos(image, pos) {
        // Checking x position
        if (image.x() < 0) {
            pos.x = 0;
        } else if (image.x() + image.width() > this.width) {
            pos.x = this.width - image.width();
        } else {
            pos.x = Math.round(image.x() / this.blockSize) * this.blockSize;
        }
        // Checking y position
        if (image.y() < 0) {
            pos.y = 0;
        } else if (image.y() + image.height() > this.height) {
            pos.y = this.height - image.height();
        } else {
            pos.y = Math.round(image.y() / this.blockSize) * this.blockSize;
        }
    }

    readAssets() {
    }
    // Map stuff

    download() {
        var element = document.createElement('a');
        element.setAttribute('href', 'XD.json');
        element.setAttribute('download', 'map.json');

        element.style.display = 'none';
        document.body.appendChild(element);

        element.click();

        document.body.removeChild(element);
    }

    strMapToObj(strMap) {
        let obj = Object.create(null);
        for (let [k, v] of strMap) {
            obj[k] = v;
        }
        return obj;
    }

    mapToJson(map) {
        return JSON.stringify(map);
    }

    // Render

    render() {
        return (
            <div>
                <p>
                    <button type="button" onClick={this.generate}>Generate map!</button>
                </p>
                <p>
                    <button type="button" onClick={this.generateRandom}>Generate random map!</button>
                </p>
                <form onSubmit={this.handleSubmit}>
                    <label>
                        Box size:
                         <input name="blockSize" type="number" value={this.state.blockSize} onChange={this.handleChange} />
                    </label>
                    <label>
                        Width:
                         <input name="width" type="number" value={this.state.width} onChange={this.handleChange} step={this.state.blockSize} />
                    </label>
                    <label>
                        Height:
                         <input name="height" type="number" value={this.state.height} onChange={this.handleChange} step={this.state.blockSize} />
                    </label>

                    <input type="button" onClick={this.handleSubmit} value="Change" />
                </form>

                <img src="./assets/box1.png" alt="box" className="box" onClick={() => this.selectBox("./assets/box1.png", "box1")} />
                <img src="./assets/box2.png" alt="box" className="box" onClick={() => this.selectBox("./assets/box2.png", "box2")} />
                <img src="./assets/piramid1.png" alt="box" className="box" onClick={() => this.selectBox("./assets/piramid1.png", "piramid1")} />
                <div
                    className="container"
                    ref={ref => {
                        this.containerRef = ref;
                    }}
                />
            </div>
        );
    }
}