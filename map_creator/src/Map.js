import React from 'react';
import * as Konva from "konva";
import * as FileSaver from 'file-saver';
import './Map.css';

export const getImages = () => {
    const images = [
        { path: require('./assets/box1.png'), type: 'box1' },
        { path: require('./assets/box2.png'), type: 'box2' },
        { path: require('./assets/piramid1.png'), type: 'piramid1' },
        { path: require('./assets/guard.png'), type: 'guard' },
        // require('./assets/box2.png'),
        // require('./assets/piramid1.png'),
        // require('./assets/guard.png')
    ];
    return images;
};

export default class Map extends React.Component {

    constructor(props) {
        super(props);
        this.stage = null;
        this.layer = null;
        this.shadowRectangle = null;
        this.blockSize = 80;
        this.imageIndex = 0;
        this.objects = [];
        this.mapObject = {};
        this.width = 800;
        this.height = 480;
        this.bindMethods = this.bindMethods.bind(this);
        this.images = getImages();
        this.freeSpots = [];
        this.oldSize = {};
        this.bindMethods();
        this.initFreeSpots();
        this.state = { width: parseInt(this.width / this.blockSize), height: this.height / this.blockSize, blockSize: this.blockSize };
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
        this.handleScale = this.handleScale.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.strMapToObj = this.strMapToObj.bind(this);
        this.readAssets = this.readAssets.bind(this);
        this.checkOverlap = this.checkOverlap.bind(this);
        this.initFreeSpots = this.initFreeSpots.bind(this);
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
        this.shadowRectangle.hide();
        const pos = { x: -1, y: -1 };
        this.shadowRectangle.setWidth(this.blockSize);
        this.shadowRectangle.setHeight(this.blockSize);
        this.checkPos(this.shadowRectangle, pos);
        this.shadowRectangle.setAttrs({
            x: pos.x,
            y: pos.y
        })
        this.layer.add(this.shadowRectangle);
        console.log(this.width, this.height);
        this.objects.forEach((entry) => {
            const pos = { x: -1, y: -1 };
            entry.setHeight(this.blockSize);
            entry.setWidth(this.blockSize);
            let newX = entry.x() / (this.oldSize.width / this.width);
            let newY = entry.y() / (this.oldSize.height / this.height);
            entry.setAttrs({ x: newX, y: newY })
            this.checkPos(entry, pos);
            entry.setAttrs({ x: pos.x, y: pos.y })
            entry.currentX = entry.x();
            entry.currentY = entry.y();
            this.layer.add(entry);
        });
        this.stage.draw();
    }

    // Click events

    generate() {
        // this.readAssets();
        // this.mapObject = { mapSize: [this.width, this.height], playerPosition: [0, 0], "box1": [], "box2": [], "piramid1": [], "guard": [] }; 
        this.mapObject = { mapSize: [this.width, this.height], playerPosition: [0, 0], gameObjects: [] };
        this.objects.forEach((entry) => {
            this.mapObject["gameObjects"].push({ "size": [entry.width(), entry.height()], "position": [entry.x(), entry.y()], type: entry.type });
            var x = this.mapToJson(this.mapObject);
            console.log(x);
            console.log("Object " + entry.type + " X: " + entry.x() + ", Y: " + entry.y());
        });
        var x = this.mapToJson(this.mapObject);
        var file = new File([x], "map.json", { type: "application/json" });
        FileSaver.saveAs(file);
    }

    generateRandom() {
        this.objects = [];
        // const items = [['./assets/box1.png', 'box1'], ['./assets/box2.png', 'box2'], ['./assets/piramid1.png', 'piramid1'], ['./assets/guard.png', 'guard']];
        for (let i = 0; i < 10; i++) {
            const randomElement = this.images[Math.floor(Math.random() * this.images.length)];
            if (this.addImage(randomElement.path, randomElement.type)) {
                continue;
            }
        }
        this.redraw();
    }

    remove(array, index) {

        if (index !== -1) {
            array.splice(index, 1);
        }
    }

    dragstart(e, tween, dragLayer) {
        const image = e.target;
        this.shadowRectangle.setAttrs({
            width: image.width(),
            height: image.height()
        });
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
        const image = e.target;
        image.moveTo(this.layer);

        image.to({
            duration: 1,
            easing: Konva.Easings.ElasticEaseOut,
            scaleX: 1,
            scaleY: 1,
            shadowOffsetX: 0,
            shadowOffsetY: 0
        });
        if (!this.checkOverlap(image, this.shadowRectangle.x(), this.shadowRectangle.y())) {
            image.position({
                x: this.shadowRectangle.x(),
                y: this.shadowRectangle.y()
            });
        } else {
            console.log("WRACAM XD")
            image.position({
                x: image.currentX,
                y: image.currentY
            });
        }
        image.currentX = image.x();
        image.currentY = image.y();
        //this.objects.push(image);
        this.stage.draw();
        this.shadowRectangle.hide();
        console.log("Image X: " + image.x() + ", Image Y: " + image.y());
    }

    selectBox(path, type) {
        this.addImage(path, type)
    }

    handleChange(event) {

        const target = event.target;
        const value = parseInt(event.target.value);
        const name = target.name;
        console.log("Setting value",name,":", value);
        this.setState({
            [name]: value
        });
    }

    handleScale(event) {
        const target = event.target;
        const value = event.target.value;
        const name = target.name;
        console.log("Setting value",name,":", value);
        this.stage.scale({
                x: value,
                y: value
            });
        this.stage.setHeight(this.height*value);
        this.stage.setWidth(this.width*value);
        this.stage.batchDraw();
    }

    handleSubmit() {
        this.oldSize = { width: this.width, height: this.height };
        this.blockSize = parseInt(this.state.blockSize);
        this.width = parseInt(this.state.width) * this.blockSize;
        this.height = parseInt(this.state.height) * this.blockSize;
        this.initFreeSpots();
        console.log("Setting this.width and this.height to: ", this.width, this.height);
        this.stage.setHeight(this.height);
        this.stage.setWidth(this.width);
        // this.setState({
        //     width: this.width,
        //     height: this.height
        // });
        this.redraw();
    }

    // Image stuff 

    imageOnLoad(imageObj, type) {
        const scale = 1;
        var index = Math.floor(Math.random() * this.freeSpots.length);
        let freeSpot = this.freeSpots[index]
        this.remove(this.freeSpots, index);
        const image = new Konva.Image({
            x: freeSpot.row * this.blockSize,
            y: freeSpot.col * this.blockSize,
            scale: {
                x: scale,
                y: scale
            },
            shadowOffset: {
                x: 0,
                y: 0
            },
            width: this.blockSize,
            height: this.blockSize,
            image: imageObj,
            draggable: true,
        });
        const pos = { x: -1, y: -1 };

        this.checkPos(image, pos);
        if (this.checkOverlap(image, image.x(), image.y())) {
            return false;
        }
        console.log(pos.x, pos.y);
        image.position({ x: pos.x, y: pos.y });

        image.type = type;
        image.src = imageObj.src;
        image.imageIndex = this.imageIndex;
        image.currentX = pos.x;
        image.currentY = pos.y;
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
            console.log("Setting rectangle position to: ", pos.x, pos.y);
            console.log("this.width, this.height: ", this.width, this.height);
            this.shadowRectangle.position({
                x: pos.x,
                y: pos.y
            });
            if (this.checkOverlap(image, this.shadowRectangle.x(), this.shadowRectangle.y())) {
                console.log("DRAGMOVE OVERLAPS");
                this.shadowRectangle.setAttrs({
                    fill: '#ff0000',
                    stroke: '#b30000'
                });
            } else {
                this.shadowRectangle.setAttrs({
                    fill: '#07fc1b',
                    stroke: '#03bc12'
                });
            }
            this.stage.batchDraw();
        });
        this.objects.push(image);
        this.layer.add(image);
        this.stage.draw();
        return true;
    }

    addImage(path, type) {

        const imageObj = new Image();
        imageObj.src = path
        imageObj.misc = { stage: this.stage, layer: this.layer };
        imageObj.onload = () => {
            return this.imageOnLoad(imageObj, type);
        };
    }

    reloadImage(image) {
        this.layer.add(image);
        this.stage.batchDraw();
    }

    checkPos(image, pos) {
        // Checking x position
        var x = parseFloat(image.x());
        var y = parseFloat(image.y());

        if (x < 0) {
            pos.x = 0;
        } else if (x + image.width() > this.width) {
            pos.x = this.width - image.width();
        } else {
            pos.x = Math.round(x / this.blockSize) * this.blockSize;
        }
        // Checking y position
        if (y < 0) {
            pos.y = 0;
        } else if (parseFloat(y) + image.height() > this.height) {
            pos.y = this.height - image.height();
        } else {
            pos.y = Math.round(y / this.blockSize) * this.blockSize;
        }
    }

    checkOverlap(image, x, y) {
        let counter = 0;
        let X = x
        let Y = y
        let A = x + image.width();
        let B = y + image.height();
        let r = false;
        this.objects.forEach((entry) => {
            if (entry !== image) {
                let X1 = entry.x();
                let A1 = entry.x() + entry.width();
                let Y1 = entry.y()
                let B1 = entry.y() + entry.height();
                if (A <= X1 || A1 <= X || B <= Y1 || B1 <= Y) {
                    r = r;
                } else {
                    r = true;
                }
            } else {
                counter++;
            }
        });
        return r;
    }

    readAssets() {
    }
    // Map stuff

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

    initFreeSpots() {
        this.freeSpots = [];
        let rows = parseInt(this.width / this.blockSize);
        let cols = parseInt(this.height / this.blockSize);
        for (let i = 0; i < rows; i++) {
            for (let j = 0; j < cols; j++) {
                this.freeSpots.push({ row: i, col: j });
            }
        }

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
                         <input name="width" type="number" value={this.state.width} onChange={this.handleChange} step="1" />
                    </label>
                    <label>
                        Height:
                         <input name="height" type="number" value={this.state.height} onChange={this.handleChange} step="1" />
                    </label>

                    <input type="button" onClick={this.handleSubmit} value="Change" />
                </form>
                <iframe name='XD' width="0" height="0" borde="0" style={{ visibility: "hidden" }}></iframe>
                <p>
                    Upload map to a server
                          <form action="http://localhost:5000/uploader" method="POST"
                        encType="multipart/form-data" target='XD'>
                        <input type="file" name="file" />
                        <input type="submit" />
                    </form>
                </p>
                <div id='images'>
                    <img src={this.images[0].path} alt="box" className="box" onClick={() => this.selectBox(this.images[0].path, this.images[0].type)} />
                    <img src={this.images[1].path} alt="box" className="box" onClick={() => this.selectBox(this.images[1].path, this.images[1].type)} />
                    <img src={this.images[2].path} alt="box" className="box" onClick={() => this.selectBox(this.images[2].path, this.images[2].type)} />
                    <img src={this.images[3].path} alt="box" className="box" onClick={() => this.selectBox(this.images[3].path, this.images[3].type)} />
                </div>
                <input name="scale" type="range" defaultValue="1.0" min="0.1" max ="2.0" step="0.01" onChange={this.handleScale}/>
                <div
                    className="container"
                    ref={ref => {
                        this.containerRef = ref;
                    }}
                />
            </div >
        );
    }
}