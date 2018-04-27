import React from 'react';
import * as Konva from "konva";
import * as FileSaver from 'file-saver';
import MuiThemeProvider from 'material-ui/styles/MuiThemeProvider';
import RaisedButton from 'material-ui/RaisedButton';
import TextField from 'material-ui/TextField';
import FineUploader from 'fine-uploader-wrappers';
import Gallery from 'react-fine-uploader'
import 'react-fine-uploader/gallery/gallery.css'
import './Map.css';

// import Upload from 'material-ui-upload/Upload';


export const getTerrains = () => {
    const terrains = [
        { path: require('./assets/card.png'), type: 'card' },
        { path: require('./assets/guard.png'), type: 'guard' },
        { path: require('./assets/tree1.png'), type: 'tree1' },
        { path: require('./assets/tree3.png'), type: 'tree3' },
        { path: require('./assets/wall1.png'), type: 'wall1' },
    ];
    return terrains;
};

export const getSingletons = () => {
    const singletons = [
        { path: require('./assets/case.png'), type: 'case' },
        { path: require('./assets/player.png'), type: 'player' },
    ];
    return singletons;
};

export const getItems = () => {
    const items = [
        { path: require('./assets/mine.png'), type: 'mine' },
        { path: require('./assets/movementPotion.png'), type: 'movementPotion' },
    ];
    return items;
};

const uploader = new FineUploader({
    options: {
        request: {
            endpoint: 'http://localhost:5000/uploader',
            method: "POST"
        }
    },
    cors: {
        //all requests are expected to be cross-domain requests
        expected: false,

        //if you want cookies to be sent along with the request
    }
});

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
        this.images = { terrains: [], items: [], singletons: [] };
        this.images.terrains = getTerrains();
        this.images.items = getItems();
        this.images.singletons = getSingletons();
        this.freeSpots = [];
        this.oldSize = { width: this.width, height: this.height };
        this.bindMethods();
        this.initFreeSpots();
        this.state = { width: parseInt(this.width / this.blockSize, 10), height: this.height / this.blockSize, blockSize: this.blockSize };
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
        console.log("this.oldSize.width this.oldSize.height:", this.oldSize.width, this.oldSize.height);
        console.log("this.width this.height:", this.width, this.height);
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
        // console.log(this.width, this.height);
        let objectsToDelete = [];
        this.objects.forEach((entry) => {
            const pos = { x: -1, y: -1 };
            entry.setHeight(this.blockSize);
            entry.setWidth(this.blockSize);
            let change = false;
            if (this.oldSize.width !== this.width || this.oldSize.height !== this.height) {
                change = true;
            }
            // let newX = entry.x() / (this.oldSize.width / this.width);
            // let newY = entry.y() / (this.oldSize.height / this.height);
            // entry.setAttrs({ x: newX, y: newY })
            this.checkPos(entry, pos, change);
            if (pos.x === -1 || pos.y === -1) {
                objectsToDelete.push(entry);
            } else {
                entry.setAttrs({ x: pos.x, y: pos.y })
                entry.currentX = entry.x();
                entry.currentY = entry.y();
                this.layer.add(entry);
            }
        });
        for (let i = 0; i < objectsToDelete.length; i++) {
            delete this.objects[objectsToDelete[i].imageIndex];
        }
        this.stage.draw();
    }

    // Click events

    generate() {
        this.mapObject = { mapSize: [this.width, this.height], playerPosition: [0, 0], treasurePosition: [0, 0], gameObjects: [] };
        this.objects.forEach((entry) => {
            if (entry.type === "player") {
                this.mapObject["playerPosition"] = [entry.x(), entry.y()];
            } else if (entry.type === "case") {
                this.mapObject["treasurePosition"] = [entry.x(), entry.y()];
            } else {
                this.mapObject["gameObjects"].push({ "size": [entry.width(), entry.height()], "position": [entry.x(), entry.y()], type: entry.type });
            }
            var x = this.mapToJson(this.mapObject);
            console.log(x);
            console.log("Object " + entry.type + " X: " + entry.x() + ", Y: " + entry.y());
        });
        var x = this.mapToJson(this.mapObject);
        var file = new File([x], "gameData.json", { type: "application/json" });
        FileSaver.saveAs(file);
    }

    generateRandom() {
        this.objects = [];
        // Adding player and the treasure
        this.addImage(this.images.singletons[0].path, this.images.singletons[0].type);
        this.addImage(this.images.singletons[1].path, this.images.singletons[1].type);
        // Adding items
        for (let i = 0; i < 3; i++) {
            const randomElement = this.images.items[Math.floor(Math.random() * this.images.items.length)];
            if (this.addImage(randomElement.path, randomElement.type)) {
                continue;
            }
        }
        for (let i = 0; i < 10; i++) {
            const randomElement = this.images.terrains[Math.floor(Math.random() * this.images.terrains.length)];
            if (this.addImage(randomElement.path, randomElement.type)) {
                continue;
            }
        }
        this.oldSize = { width: this.width, height: this.height };
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
        // console.log("Image X: " + image.x() + ", Image Y: " + image.y());
    }

    selectBox(path, type) {
        this.addImage(path, type)
    }

    handleChange(event) {

        const target = event.target;
        const value = parseInt(event.target.value, 10);
        const name = target.name;
        console.log("Setting value", name, ":", value);
        this.setState({
            [name]: value
        });
    }

    handleScale(event) {
        const target = event.target;
        const value = event.target.value;
        const name = target.name;
        console.log("Setting value", name, ":", value);
        this.stage.scale({
            x: value,
            y: value
        });
        this.stage.setHeight(this.height * value);
        this.stage.setWidth(this.width * value);
        this.stage.draw();
    }

    handleSubmit() {
        this.oldSize = { width: this.width, height: this.height };
        this.blockSize = parseInt(this.state.blockSize, 10);
        this.width = parseInt(this.state.width, 10) * this.blockSize;
        this.height = parseInt(this.state.height, 10) * this.blockSize;
        this.initFreeSpots();
        console.log("Setting this.width and this.height to: ", this.width, this.height);
        let scale = this.stage.scaleX();
        this.stage.setHeight(this.height * scale);
        this.stage.setWidth(this.width * scale);
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

        this.checkPos(image, pos, false);
        if (this.checkOverlap(image, image.x(), image.y())) {
            return false;
        }
        // console.log(pos.x, pos.y);
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
            this.checkPos(image, pos, false);
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
        this.remove(this.freeSpots, index);
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

    checkPos(image, pos, change) {
        // Checking x position
        var x = parseFloat(image.x());
        var y = parseFloat(image.y());

        if (x < 0) {
            pos.x = 0;
        } else if (x + image.width() > this.width) {
            if (change) {
                pos.x = -1;
            } else {
                pos.x = this.width - image.width();
            }

        } else {
            pos.x = Math.round(x / this.blockSize) * this.blockSize;
        }
        // Checking y position
        if (y < 0) {
            pos.y = 0;
        } else if (parseFloat(y) + image.height() > this.height) {
            if (change) {
                pos.y = -1;
            } else {
                pos.y = this.height - image.height();
            }
        } else {
            pos.y = Math.round(y / this.blockSize) * this.blockSize;
        }
    }

    checkOverlap(image, x, y) {
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
                if (!(A <= X1 || A1 <= X || B <= Y1 || B1 <= Y)) {
                    r = true;
                }
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
        let rows = parseInt(this.width / this.blockSize, 10);
        let cols = parseInt(this.height / this.blockSize, 10);
        for (let i = 0; i < rows; i++) {
            for (let j = 0; j < cols; j++) {
                this.freeSpots.push({ row: i, col: j });
            }
        }

    }

    // Render
    testUpload() {
        this.upload.click();
        let f = this.upload.value;
        f = f.replace(/.*[/\\]/, '');
        console.log(f);
    }

    render() {
        return (
            <MuiThemeProvider /*muiTheme={getMuiTheme(darkBaseTheme)}*/>
                <div className="main-div">
                    <RaisedButton className="strange-button" label="Generate map" primary={true} onClick={this.generate}></RaisedButton>
                    <br />
                    <RaisedButton className="strange-button" label="Generate random map" primary={true} onClick={this.generateRandom}></RaisedButton>
                    <br />
                    <RaisedButton className="strange-button" label="List maps" primary={true} onClick={this.listMaps}></RaisedButton>
                    <form onSubmit={this.handleSubmit}>
                        <label>
                            Box size:
                            <TextField className="text-field-sizes" hintText="Box size" name="blockSize" value={this.state.blockSize} onChange={this.handleChange}></TextField>
                        </label>
                        <label>
                            Width:
                            <TextField className="text-field-sizes" hintText="Width" name="width" value={this.state.blockSize} onChange={this.handleChange}></TextField>
                        </label>
                        <label>
                            Height:
                            <TextField className="text-field-sizes" hintText="Height" name="height" value={this.state.blockSize} onChange={this.handleChange}></TextField>
                        </label>

                        <RaisedButton className="strange-button" label="Change" secondary={true} onClick={this.handleSubmit} />
                    </form>
                    <iframe name='hiddenframe' title='hiddenframe' width="0" height="0" border="0" style={{ visibility: "hidden" }}></iframe>
                    {/* Upload map to a server */}
                    <Gallery uploader={uploader} width="100" height="200" />
                    {/* <form action="http://localhost:5000/uploader" method="POST"
                            encType="multipart/form-data" target='hiddenframe'>
                            <RaisedButton className="strange-button" label="Browse" primary={true} onClick={(e) => this.testUpload()}></RaisedButton>
                            <TextField readOnly={true}></TextField>
                            <input type="file" name="file" style={{ display: "none" }} ref={(ref) => this.upload = ref} />
                            <RaisedButton className="strange-button" label="Submit" primary={true} type="submit" />
                    </form> */}
                    <p>{/* <div id='images'> */}
                        <img src={this.images.terrains[0].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[0].path, this.images.terrains[0].type)} />
                        <img src={this.images.terrains[1].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[1].path, this.images.terrains[1].type)} />
                        <img src={this.images.terrains[2].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[2].path, this.images.terrains[2].type)} />
                        <img src={this.images.terrains[3].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[3].path, this.images.terrains[3].type)} />
                        <img src={this.images.terrains[4].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[4].path, this.images.terrains[4].type)} />
                        <img src={this.images.items[0].path} alt="box" className="box" onClick={() => this.selectBox(this.images.items[0].path, this.images.items[0].type)} />
                        <img src={this.images.items[1].path} alt="box" className="box" onClick={() => this.selectBox(this.images.items[1].path, this.images.items[1].type)} />
                        <img src={this.images.singletons[0].path} alt="box" className="box" onClick={() => this.selectBox(this.images.singletons[0].path, this.images.singletons[0].type)} />
                        <img src={this.images.singletons[1].path} alt="box" className="box" onClick={() => this.selectBox(this.images.singletons[1].path, this.images.singletons[1].type)} />
                    </p>{/* </div> */}
                    <input name="scale" type="range" defaultValue="1.0" min="0.1" max="2.0" step="0.01" onChange={this.handleScale} />
                    <div
                        className="container"
                        text-align="center"
                        ref={ref => {
                            this.containerRef = ref;
                        }}
                    />
                </div >
            </MuiThemeProvider>
        );
    }
}