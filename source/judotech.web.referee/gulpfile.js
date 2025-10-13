import gulp from 'gulp';

import yargs from 'yargs'
import { hideBin } from 'yargs/helpers'
const argv = yargs(hideBin(process.argv))
  .option('environment', {
    alias: 'e',
    type: 'string',
    description: 'What environment to build for [production || test]',
    default: 'development', 
    demandOption: false
  }).argv;
  console.log(`Environment: ${argv.environment}`);

import clean from 'gulp-clean';
gulp.task('clean', function () {
  return gulp.src('./public', { read: false, allowEmpty: true })
      .pipe(clean());
});

import pug from 'gulp-pug';
gulp.task('pug', function () {
  return gulp.src('./source/pug/pages/**/*.pug')
      .pipe(pug({ pretty: true }))
      .pipe(gulp.dest('./public'));
});

import fs from 'node:fs';
import concat from 'gulp-concat'
import uglify from 'gulp-uglify'
import rename from 'gulp-rename'
gulp.task('scripts', function () 
{
  const SOURCE_DIR = "source/javascripts/";
  const OUTPUT_DIR = "public/javascripts/";
  const folders = fs.readdirSync(SOURCE_DIR, { withFileTypes: true })
    .filter(dirent => dirent.isDirectory() && dirent.name != "config") 
    .map(dirent => dirent.name);

  const tasks = folders.map(folder => {
      var stream = gulp.src(`${SOURCE_DIR}${folder}/*.js`) 
          .pipe(concat(`${folder}.js`)) 
          //.pipe(rename({ extname: ".min.js" })) 
        if (argv.environment=='production') stream = stream.pipe(uglify())
      return stream.pipe(gulp.dest(OUTPUT_DIR));
  });
  tasks.push(gulp.src('./source/javascripts/config/'+argv.environment+'.js').pipe(concat('config.js')).pipe(gulp.dest('./public/javascripts/')));
  return Promise.all(tasks);
});

import changed from 'gulp-changed'
import autoprefixer from 'gulp-autoprefixer'
import csso from 'gulp-csso'
gulp.task('styles', function () {
  const source = './source/stylesheets/*.css';
  return gulp.src(source)
      .pipe(changed(source))
      .pipe(autoprefixer({
          overrideBrowserslist: ['last 2 versions'],
          cascade: false
      }))
      .pipe(rename({
          extname: '.min.css'
      }))
      .pipe(csso())
      .pipe(gulp.dest('./public/stylesheets/'))
    });

gulp.task('images', function () {
  return gulp.src('./source/images/**/*.*',  { encoding: false })
      .pipe(gulp.dest('./public/images'));
});

gulp.task('watch', function () {
  gulp.watch('source/pug/**/*.pug', gulp.series('pug'));
  gulp.watch('source/stylesheets/**/*.css', gulp.series('styles'));
  gulp.watch('source/javascripts/**/*.js', gulp.series('scripts'));
  gulp.watch('source/images/**/*.*', gulp.series('images'));
});

gulp.task('default', gulp.series('clean', 'pug','styles','scripts','images', function (done) {
  done();
}));
