import React, { Component } from 'react';
import Map from "./Map";
import logo from './logo.svg';
import box from './box.png'
import './App.css';

class App extends Component {
    constructor(props) {
        super(props);
        this.state = { selectedElement: null }
    }
    render() {
        return (
            <div className="App">
                <header className="App-header">
                    <img src={logo} className="App-logo" alt="logo" />
                    <h1 className="App-title">Welcome to React</h1>
                </header>
                <p className="App-intro">
                    To get started, edit <code>src/App.js</code> and save to reload.
                </p>
                <img src={box} alt="box" width="100" height="100" onClick={selectBox} />
                <Map selectedElement={this.state.selectedElement} />
            </div>
        );
    }
    selectBox() {
        this.setState({ selectedElement: "./box.png" });
    }
}
export default App;
