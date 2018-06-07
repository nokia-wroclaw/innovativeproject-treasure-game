import React from 'react';
import * as Konva from "konva";
import * as FileSaver from 'file-saver';
import './Map.css';
import MuiThemeProvider from '@material-ui/core/styles/MuiThemeProvider';
import Button from '@material-ui/core/Button';
// import Slider from '@material-ui/core/Slider';
import TextField from '@material-ui/core/TextField';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import axios from 'axios';
import 'react-fine-uploader/gallery/gallery.css'

// import Upload from 'material-ui-upload/Upload';
const styles = {
    root: {
        display: 'flex',
    },
    button: {
        margin: "20px",
    },
    list: {
        width: "50%",
        height: 200,
        overflowY: 'auto',
        display: 'block'
    },
    listItem: {
        textAlign: 'center'
    },
    sizesForm: {
        textAlign: 'center',
        width: "50%",
        left: "50%"
    },
};


const getTerrains = () => {
    const terrains = [
        { path: require('./assets/card.png'), type: 'card' },
        { path: require('./assets/guard.png'), type: 'guard' },
        { path: require('./assets/tree1.png'), type: 'tree1' },
        { path: require('./assets/tree3.png'), type: 'tree3' },
        { path: require('./assets/wall1.png'), type: 'wall1' },
    ];
    return terrains;
};

const getSingletons = () => {
    const singletons = [
        { path: require('./assets/case.png'), type: 'case' },
        { path: require('./assets/player.png'), type: 'player' },
    ];
    return singletons;
};

const getItems = () => {
    const items = [
        { path: require('./assets/mine.png'), type: 'mine' },
        { path: require('./assets/movementPotion.png'), type: 'movementPotion' },
    ];
    return items;
};


export default class Map extends React.Component {

    constructor(props) {
        super(props);
        console.log(this.props.firstLoad);
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
        this.playerOnMap = false;
        this.treasureOnMap = false;
        this.bindMethods();
        this.initFreeSpots();
        this.state = { width: parseInt(this.width / this.blockSize, 10), height: this.height / this.blockSize, blockSize: this.blockSize, scale: 1.0, maps: null };
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
        });

    }

    // Misc

    bindMethods() {
        this.selectBox = this.selectBox.bind(this);
        this.dragstart = this.dragstart.bind(this);
        this.dragend = this.dragend.bind(this);
        this.imageOnLoad = this.imageOnLoad.bind(this);
        this.addImage = this.addImage.bind(this);
        this.addImages = this.addImages.bind(this);
        this.drawGrid = this.drawGrid.bind(this);
        this.redraw = this.redraw.bind(this);
        this.generateMap = this.generateMap.bind(this);
        this.downloadMap = this.downloadMap.bind(this);
        this.generateRandom = this.generateRandom.bind(this);
        this.checkPos = this.checkPos.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleSlider = this.handleSlider.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.strMapToObj = this.strMapToObj.bind(this);
        this.readAssets = this.readAssets.bind(this);
        this.checkOverlap = this.checkOverlap.bind(this);
        this.initFreeSpots = this.initFreeSpots.bind(this);
        this.getMaps = this.getMaps.bind(this);
        this.makeList = this.makeList.bind(this);
        this.mapFromListElement = this.mapFromListElement.bind(this);
        this.mapListClick = this.mapListClick.bind(this);
        this.setMapSize = this.setMapSize.bind(this);
        this.uploadMap = this.uploadMap.bind(this);
        this.clearMap = this.clearMap.bind(this);
        this.clearObjectsArray = this.clearObjectsArray.bind(this);
        this.claimFreeSpot = this.claimFreeSpot.bind(this);
        this.mapList = [];
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

    clearObjectsArray() {
        this.objects = [];
        this.imageIndex = 0;
        this.initFreeSpots();
    }

    clearMap() {
        this.layer.removeChildren();
        this.drawGrid();
    }

    redraw() {
        this.clearMap();
        var tempObjects = this.objects.slice();
        this.clearObjectsArray();
        // this.initFreeSpots();
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
        let objectsToDelete = [];
        this.addImages(tempObjects);
        // tempObjects.forEach((entry) => {
        //     // const pos = { x: -1, y: -1 };
        //     // entry.setHeight(this.blockSize);
        //     // entry.setWidth(this.blockSize);
        //     // let change = false;
        //     // if (this.oldSize.width !== this.width || this.oldSize.height !== this.height) {
        //     //     change = true;
        //     // }
        //     // this.checkPos(entry, pos, change);
        //     // if (pos.x === -1 || pos.y === -1) {
        //     //     objectsToDelete.push(entry);
        //     // } else {
        //     //     entry.setAttrs({ x: pos.x, y: pos.y })
        //     //     entry.currentX = entry.x();
        //     //     entry.currentY = entry.y();
        //     //     this.layer.add(entry);
        //     // }
        //     console.log(entry.src, entry.type, { x: entry.x(), y: entry.y() });
        //     this.addImages(entry);
        // });
        for (let i = 0; i < objectsToDelete.length; i++) {
            delete this.objects[objectsToDelete[i].imageIndex];
        }
        // this.stage.draw();
    }

    // Click events
    downloadMap() {
        const map = this.generateMap();
        const file = new File([map], "gameData.json", { type: "application/json" });
        FileSaver.saveAs(file);
    }

    generateMap() {
        this.mapObject = { mapSize: [this.width, this.height], playerPosition: [0, 0], treasurePosition: [0, 0], gameObjects: [] };
        this.objects.forEach((entry) => {
            if (entry.type === "player") {
                this.mapObject["playerPosition"] = [entry.x(), entry.y()];
            } else if (entry.type === "case") {
                this.mapObject["treasurePosition"] = [entry.x(), entry.y()];
            } else {
                this.mapObject["gameObjects"].push({ "size": [entry.width(), entry.height()], "position": [entry.x(), entry.y()], type: entry.type });
            }
        });
        var d = new Date();
        var date = d.toLocaleString("en-gb");
        this.mapObject["createTime"] = date;
        const json = this.mapToJson(this.mapObject);
        return json;
    }

    generateRandom() {
        this.clearMap();
        this.clearObjectsArray();
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
    }

    getMaps() {
        const getter = axios.create({
            baseURL: `http://localhost:5000/maps`
        });
        getter.get().then(res => res.data).then(data => {
            this.setState({ maps: data["maps"] }, () => { this.makeList() });
        });
    }

    uploadMap() {
        const map = this.generateMap();
        axios.post('http://localhost:5000/uploader_json', map).then(res => res.data).then(data => {
        });
    }

    makeList() {
        var maps = this.state.maps;
        for (let i = 0; i < maps.length; i++) {
            this.mapList.push(<ListItem button key={i} style={styles.listItem} onClick={(event) => this.mapListClick(event, maps[i])} ><ListItemText primary={maps[i]} /></ListItem>);
        }
        this.forceUpdate();
    }

    mapListClick(event, mapName) {
        this.mapFromListElement(mapName);
    }

    mapFromListElement(mapName) {
        const getter = axios.create({
            baseURL: `http://localhost:5000/map/` + mapName
        });
        getter.get().then(res => res.data).then(data => {
            this.clearMap();
            this.clearObjectsArray();
            let gameObjects = data["gameObjects"];
            for (let i = 0; i < gameObjects.length; i++) {
                let entry = gameObjects[i];
                let object = this.images.terrains.find(function (v) { return v["type"] === entry.type });
                if (!(object == null)) {
                    let position = { x: entry.position[0], y: entry.position[1] };
                    this.addImage(object.path, object.type, position);
                    continue;
                }
                object = this.images.items.find(function (v) { return v["type"] === entry.type });
                if (!(object == null)) {
                    let position = { x: entry.position[0], y: entry.position[1] };
                    this.addImage(object.path, object.type, position);
                    continue;
                }
            }
            let playerPosition = data["playerPosition"];
            let position = { x: playerPosition[0], y: playerPosition[1] };
            this.addImage(require('./assets/player.png'), "player", position);
            let treasurePostion = data["treasurePosition"];
            position = { x: treasurePostion[0], y: treasurePostion[1] };
            this.addImage(require('./assets/case.png'), "case", position);
            let mapSize = data["mapSize"];
            this.setMapSize(mapSize[0], mapSize[1], true);
            // this.redraw();
        });
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
    }

    selectBox(path, type) {
        this.addImage(path, type)
    }

    handleChange(event) {
        const target = event.target;
        const value = parseInt(event.target.value, 10);
        const name = target.name;
        this.setState({
            [name]: value
        });
    }



    handleSubmit() {
        this.setMapSize(this.state.width, this.state.height);
        this.blockSize = parseInt(this.state.blockSize, 10);
        this.redraw();
    }


    setMapSize(width, height, fromFile) {
        if (fromFile != null) {
            this.oldSize = { width: this.width, height: this.height };
            this.width = parseInt(width, 10);
            this.height = parseInt(height, 10);
        }
        else {
            this.oldSize = { width: this.width, height: this.height };
            this.width = parseInt(width, 10) * this.blockSize;
            this.height = parseInt(height, 10) * this.blockSize;
        }
        let scale = this.stage.scaleX();
        this.stage.setHeight(this.height * scale);
        this.stage.setWidth(this.width * scale);
    }
    // Image stuff 

    imageOnLoad(imageObj, type, position) {
        let loadedFromFile = false;
        let pos = {};
        if (position != null) {
            pos = { x: position.x, y: position.y };
            loadedFromFile = true;
        }
        else {
            pos = { x: -1, y: -1 };
        }
        var scale = 1;
        var index = Math.floor(Math.random() * this.freeSpots.length);
        var freeSpot = this.freeSpots[index];
        if (freeSpot === undefined) {
            return;
        }
        var image = new Konva.Image({
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

        if (!loadedFromFile) {
            this.checkPos(image, pos, false);
            if (this.checkOverlap(image, image.x(), image.y())) {
                return false;
            }
        }
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
            if (image.type === "case") {
                this.treasureOnMap = false;
            } else if (image.type === "player") {
                this.playerOnMap = false;
            }
            this.redraw();
        });
        image.on('dragmove', () => {
            const pos = { x: -1, y: -1 };
            this.checkPos(image, pos, false);
            this.shadowRectangle.position({
                x: pos.x,
                y: pos.y
            });
            if (this.checkOverlap(image, this.shadowRectangle.x(), this.shadowRectangle.y())) {
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
        this.remove(this.freeSpots, index);
        return true;
    }

    addImages(images) {
        images.forEach(function (image) {
            const imageObj = new Image();
            imageObj.src = image.src
            imageObj.misc = { stage: this.stage, layer: this.layer };
            this.imageOnLoad(imageObj, image.type, { x: image.x(), y: image.y() });
        }, this);
        this.stage.draw();
    }

    addImage(path, type, position) {
        if (type === "case") {
            if (this.treasureOnMap) {
                return;
            } else {
                this.treasureOnMap = true;
            }
        } else if (type === "player") {
            if (this.playerOnMap) {
                return;
            } else {
                this.playerOnMap = true;
            }
        }

        const imageObj = new Image();
        imageObj.src = path
        imageObj.misc = { stage: this.stage, layer: this.layer };
        imageObj.onload = () => {
            const ret = this.imageOnLoad(imageObj, type, position);
            this.stage.draw();
            return ret;
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

    claimFreeSpot(index) {
        this.remove(this.freeSpots, index);
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

    handleSlider(event, value) {
        this.setState({ scale: value });
        this.stage.scale({
            x: value,
            y: value
        });
        this.stage.setHeight(this.height * value);
        this.stage.setWidth(this.width * value);
        this.stage.draw();
    }

    render() {
        return (
            <MuiThemeProvider>
                <div className="main-div">
                    <div style={{ textAlign: "center" }}>
                        <Button style={styles.button} variant="raised" color="primary" onClick={this.generateRandom}>Create random map</Button>
                        <Button style={styles.button} variant="raised" color="primary" onClick={this.downloadMap}>Download map</Button>
                        <Button style={styles.button} variant="raised" color="primary" onClick={this.uploadMap}>Upload map</Button>
                        <Button style={styles.button} variant="raised" color="primary" onClick={this.getMaps}>List maps</Button>
                    </div>
                    <div style={styles.root}>
                        <form onSubmit={this.handleSubmit} style={styles.sizesForm}>
                            <label>
                                Box size:
                            <TextField style={{ margin: "10px", width: "30px" }} hintText="Box size" name="blockSize" value={this.state.blockSize} onChange={this.handleChange}></TextField>
                            </label>
                            <br />
                            <label>
                                Width:
                            <TextField style={{ margin: "10px", width: "30px" }} hintText="Width" name="width" value={this.state.width} onChange={this.handleChange}></TextField>
                            </label>
                            <br />
                            <label>
                                Height:
                            <TextField style={{ margin: "10px", width: "30px" }} hintText="Height" name="height" value={this.state.height} onChange={this.handleChange}></TextField>
                            </label>
                            <br />
                            <Button style={{ margin: "10px" }} variant="raised" color="secondary" onClick={this.handleSubmit}>Change</Button>
                        </form>
                        <List style={styles.list} id="mapsList" name="mapsList">
                            {this.mapList}
                        </List>
                    </div>
                    <p style={{ textAlign: "center" }}>
                        <img src={this.images.terrains[0].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[0].path, this.images.terrains[0].type)} />
                        <img src={this.images.terrains[1].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[1].path, this.images.terrains[1].type)} />
                        <img src={this.images.terrains[2].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[2].path, this.images.terrains[2].type)} />
                        <img src={this.images.terrains[3].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[3].path, this.images.terrains[3].type)} />
                        <img src={this.images.terrains[4].path} alt="box" className="box" onClick={() => this.selectBox(this.images.terrains[4].path, this.images.terrains[4].type)} />
                        <img src={this.images.items[0].path} alt="box" className="box" onClick={() => this.selectBox(this.images.items[0].path, this.images.items[0].type)} />
                        <img src={this.images.items[1].path} alt="box" className="box" onClick={() => this.selectBox(this.images.items[1].path, this.images.items[1].type)} />
                        <img src={this.images.singletons[0].path} alt="box" className="box" onClick={() => this.selectBox(this.images.singletons[0].path, this.images.singletons[0].type)} />
                        <img src={this.images.singletons[1].path} alt="box" className="box" onClick={() => this.selectBox(this.images.singletons[1].path, this.images.singletons[1].type)} />
                    </p>
                    <div>
                        {/* <Slider min={0.1} max={2.0} defaultValue={1.0} value={this.state.scale} onChange={this.handleSlider} className="slider" /> */}
                    </div>
                    <div
                        className="container"
                        text-align="center"
                        ref={ref => {
                            this.containerRef = ref;
                        }}
                        style={{ display: "flex" }}
                    >
                    </div>

                </div >
            </MuiThemeProvider>
        );
    }
}