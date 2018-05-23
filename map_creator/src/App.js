import React, { Component } from 'react';
import './App.css';
import Map from "./Map";
import Misc from "./Misc";
import MuiThemeProvider from '@material-ui/core/styles/MuiThemeProvider';
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import AppBar from '@material-ui/core/AppBar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';

class App extends Component {

    render() {
        var styles = {
            appBar: {
                flexWrap: 'wrap'
            },
            tabs: {
                width: '100%'
            }
        }
        const value = 0;
        return (
            <Router history={Router}>
                <div>
                    <MuiThemeProvider>
                        <AppBar showMenuIconButton={false} style={styles.appBar} position="static">
                            <Tabs onChange={this.changeTab} value={value}>
                                <Tab label="Editor" component={Link} to="/" />
                                <Tab label="Misc" component={Link} to="/misc" />
                            </Tabs>
                        </AppBar>
                    </MuiThemeProvider>
                    <Route exact path="/" component={Map} />
                    <Route path="/misc" component={Misc} />
                </div>
            </Router >
        );
    }
}
export default App;
